﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Models;
using Arma3TacMapWebApp.Services.GameMapStorage;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;
using BAMCIS.GeoJSON;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Arma3TacMapWebApp.Controllers
{

    [Authorize(Policy = "LoggedUser")]
    public class TacMapsController : Controller
    {
        private readonly Arma3TacMapContext _context;
        private readonly IGameMapStorageService _mapInfos;
        private readonly MapService _mapSvc;
        private readonly IMapPreviewService _preview;
        private readonly IAuthorizationService _authorizationService;

        public TacMapsController(Arma3TacMapContext context, IGameMapStorageService mapInfos, MapService mapSvc, IMapPreviewService preview, IAuthorizationService authorizationService)
        {
            _context = context;
            _mapInfos = mapInfos;
            _mapSvc = mapSvc;
            _preview = preview;
            _authorizationService = authorizationService;
        }

        // GET: TacMaps
        public async Task<IActionResult> Index()
        {
            var games = new List<GameJsonBase>();
            var allGames = await _mapInfos.GetGames();
            foreach (var game in allGames)
            {
                var maps = await _mapInfos.GetMaps(game.Name!);
                if (maps.Length > 0)
                {
                    games.Add(game);
                }
            }

            var vm = new IndexViewModel()
            {
                Games = games,
                TacMaps = await _mapSvc.GetUserMaps(User)
            };
            foreach (var map in vm.TacMaps)
            {
                map.TacMap.MapInfos = await _mapInfos.GetMapBase(map.TacMap.GameName, map.TacMap.WorldName);
            }
            return View(vm);
        }

        // GET: TacMaps/Create
        public async Task<IActionResult> Create(string? worldName, string gameName = "arma3")
        {
            var user = await _mapSvc.GetUser(User);
            if (user == null)
            {
                return Forbid();
            }
            await PrepareOrbatDropdown(user);
            var maps = (await _mapInfos.GetMaps(gameName)).OrderBy(m => m.EnglishTitle).ToList();
            if (string.IsNullOrEmpty(worldName) && maps.Count > 0)
            {
                worldName = maps.First().Name;
            }
            ViewBag.Maps = maps;
            return View(new TacMap()
            {
                WorldName = worldName ?? string.Empty,
                GameName = gameName,
                Label = "Carte tactique sans nom",
                OwnerUserID = user.UserID
            });
        }

        private async Task PrepareOrbatDropdown(User user)
        {
            var userId = user?.UserID;
            var orbatList = await _context.Orbats
                .Where(o => o.OwnerUserID == userId || o.Visibility != OrbatVisibility.Default)
                .Include(o => o.Owner)
                .OrderBy(o => o.Visibility)
                .ThenByDescending(o => o.Created)
                .ToListAsync();
            var defaultGroup = new SelectListGroup() { Name = "User defined" };
            var publicGroup = new SelectListGroup() { Name = "Public" };
            ViewBag.OrbatID = orbatList.Select(o => new SelectListItem()
            {
                Value = o.OrbatID.ToString(),
                Text = o.Label,
                Group = o.Visibility == OrbatVisibility.Default ? defaultGroup : publicGroup
            }).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Label,WorldName,GameName,FriendlyOrbatID,HostileOrbatID")] TacMap tacMap)
        {
            if (ModelState.IsValid)
            {
                var map = await _mapSvc.CreateMap(User, tacMap.WorldName, tacMap.GameName, tacMap.Label, null, tacMap.FriendlyOrbatID, tacMap.HostileOrbatID);
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id = map.TacMapID });
            }
            var maps = (await _mapInfos.GetMaps(tacMap.GameName)).OrderBy(m => m.EnglishTitle).ToList();
            ViewBag.Maps = maps;
            return View(tacMap);
        }

        // GET: TacMaps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);
            if (tacMap == null || tacMap.ParentTacMapID != null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            if (user == null || tacMap.OwnerUserID != user.UserID)
            {
                return Forbid();
            }
            await PrepareOrbatDropdown(user);
            return View(tacMap);
        }

        // POST: TacMaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TacMapID,Label,WorldName,GameName,FriendlyOrbatID,HostileOrbatID")] TacMap edited)
        {
            if (id != edited.TacMapID)
            {
                return NotFound();
            }
            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);
            if (tacMap == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            if (user == null || tacMap.OwnerUserID != user.UserID || tacMap.ParentTacMapID != null)
            {
                return Forbid();
            }
            tacMap.FriendlyOrbatID = edited.FriendlyOrbatID;
            tacMap.HostileOrbatID = edited.HostileOrbatID;
            tacMap.Label = edited.Label;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tacMap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TacMapExists(edited.TacMapID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id = edited.TacMapID });
            }
            await PrepareOrbatDropdown(user);
            return View(tacMap);
        }

        // GET: TacMaps/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);
            if (tacMap == null || tacMap.ParentTacMapID != null)
            {
                return NotFound();
            }

            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID)
            {
                return Forbid();
            }
            return View(tacMap);
        }

        // POST: TacMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tacMap = await _context.TacMaps.FindAsync(id);
            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID || tacMap.ParentTacMapID != null)
            {
                return Forbid();
            }
            _context.TacMaps.Remove(tacMap);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Forget(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            var tacMapAccess = await _context.TacMapAccesses
                .Include(t => t.TacMap).ThenInclude(m => m.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id && m.UserID == user.UserID);
            if (tacMapAccess == null)
            {
                return NotFound();
            }
            if (tacMapAccess.TacMap.OwnerUserID == user.UserID)
            {
                return RedirectToAction(nameof(Delete), new { id });
            }
            return View(tacMapAccess.TacMap);
        }

        [HttpPost, ActionName("Forget")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetConfirmed(int id)
        {
            var user = await _mapSvc.GetUser(User);
            var tacMapAccess = await _context.TacMapAccesses.FirstOrDefaultAsync(m => m.TacMapID == id && m.UserID == user.UserID);
            if (tacMapAccess == null)
            {
                return NotFound();
            }
            _context.TacMapAccesses.Remove(tacMapAccess);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Export(int id)
        {
            var mapAccess = await _mapSvc.GrantReadAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }

            return View(new ExportViewModel()
            {
                TacMap = mapAccess.TacMap,
                Access = mapAccess,
                Layers = new[] { mapAccess.TacMap }.Concat(await _context.TacMaps.Where(m => m.ParentTacMapID == mapAccess.TacMap.TacMapID).ToListAsync()).ToList(),
                Script = MapExporter.GetSqf(await _mapSvc.GetMarkers(id, true))
            });
        }

        [HttpPost]
        public async Task<IActionResult> Export(int id, int[] tacMapIds, int channel = 0)
        {
            var mapAccess = await _mapSvc.GrantReadAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }

            return View(new ExportViewModel()
            {
                TacMap = mapAccess.TacMap,
                Access = mapAccess,
                Layers = new[] { mapAccess.TacMap }.Concat(await _context.TacMaps.Where(m => m.ParentTacMapID == mapAccess.TacMap.TacMapID).ToListAsync()).ToList(),
                Script = MapExporter.GetSqf((await _mapSvc.GetMarkers(id, true)).Where(m => tacMapIds.Contains(m.LayerId)), channel),
                IsPartialExport = true,
                Channel = channel
            });
        }

        [HttpGet]
        public async Task<IActionResult> ExportLayers(int id)
        {
            var mapAccess = await _mapSvc.GrantReadAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }

            return View(new ExportLayersViewModel()
            {
                TacMap = mapAccess.TacMap,
                Access = mapAccess,
                Layers = new[] { mapAccess.TacMap }.Concat(await _context.TacMaps.Where(m => m.ParentTacMapID == mapAccess.TacMap.TacMapID).ToListAsync()).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> ExportLayers(int id, int[] tacMapIds)
        {
            var mapAccess = await _mapSvc.GrantReadAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }

            GeoJsonConfig.IgnorePositionValidation();

            var map = await _mapSvc.GetInitialData(User, new MapId() { TacMapID = id, IsReadOnly = true });

            var features = new List<Feature>();
            var exported = new List<int>();
            foreach(var layer in map.InitialLayers)
            {
                if (tacMapIds.Contains(layer.Id))
                {
                    exported.Add(layer.Id);
                    foreach(var marker in map.InitialMarkers.Where(m => m.LayerId == layer.Id))
                    {
                        var data = MarkerData.Deserialize(marker.MarkerData);
                        Geometry geometry = null;

                        switch(data.type)
                        {
                            case "basic":
                            case "mil":
                                geometry = new BAMCIS.GeoJSON.Point(new Position(data.pos[1], data.pos[0])); 
                                break;
                            case "line":
                            case "measure":
                                geometry = new LineString(ToLine(data.pos));
                                break;
                            case "mission":
                                geometry = new MultiPoint(ToLine(data.pos));
                                break;
                        }

                        if (geometry != null)
                        {
                            var properties = new Dictionary<string, dynamic>() {
                                { "tacmap:type", data.type },
                                { "tacmap:symbol", data.symbol },
                                { "tacmap:config", data.config }
                            };
                            if ( data.scale != null && data.scale != 1)
                            {
                                properties.Add("tacmap:scale", data.scale.Value);
                            }
                            features.Add(new Feature(geometry, properties));
                        }
                    }
                }
            }

            var export = new FeatureCollection(features);
            return File(Encoding.UTF8.GetBytes(export.ToJson()), "application/vnd.geo+json", "layers-" + string.Join(",", exported) + ".geojson");
        }

        [HttpGet]
        public async Task<IActionResult> ImportLayer(int id)
        {
            var mapAccess = await _mapSvc.GrantWriteAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }
            return View(new ImportLayerViewModel() { TacMap = mapAccess.TacMap, Access = mapAccess });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportLayer(int id, [Bind("Label")] ImportLayerViewModel vm, IFormFile geoJson)
        {
            var mapAccess = await _mapSvc.GrantWriteAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }
            vm.TacMap = mapAccess.TacMap;
            vm.Access = mapAccess;

            FeatureCollection? collection = null;
            GeoJsonConfig.IgnorePositionValidation();
            try
            {
                using (var reader = new StreamReader(geoJson.OpenReadStream()))
                {
                    collection = FeatureCollection.FromJson(reader.ReadToEnd());
                }
            }
            catch
            {
                ModelState.AddModelError("geoJson", "Invalid file");
            }
            if (collection != null)
            {
                var mapId = new MapId() { TacMapID = mapAccess.TacMapID };
                var layer = await _mapSvc.CreateLayer(User, mapId, vm.Label);
                foreach(var feature in collection.Features)
                {
                    if ( feature.Properties.TryGetValue("tacmap:type", out var type) &&
                        feature.Properties.TryGetValue("tacmap:symbol", out var symbol) &&
                        feature.Properties.TryGetValue("tacmap:config", out var config))
                    {
                        var markerData = new MarkerData();
                        markerData.type = Convert.ToString(type);
                        markerData.symbol = Convert.ToString(symbol);
                        markerData.config = ConvertToDict(config);
                        markerData.pos = ConvertToPos(feature.Geometry);
                        if (feature.Properties.TryGetValue("tacmap:scale", out var scale))
                        {
                            markerData.scale = Convert.ToDouble(scale);
                        }
                        await _mapSvc.AddMarker(User, mapId, layer.Id, MarkerData.Serialize(markerData));
                    }
                }
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id }, "showLayers");
            }
            return View(vm);
        }

        private double[] ConvertToPos(Geometry geometry)
        {
            var point = geometry as BAMCIS.GeoJSON.Point;
            if (point != null)
            {
                return new[] { point.Coordinates.Latitude, point.Coordinates.Longitude };
            }
            var line = geometry as LineString;
            if (line != null)
            {
                return line.Coordinates.SelectMany(c => new[] { c.Latitude, c.Longitude }).ToArray();
            }
            var multi = geometry as MultiPoint;
            if (multi != null)
            {
                return multi.Coordinates.SelectMany(c => new[] { c.Latitude, c.Longitude }).ToArray();
            }
            return new double[0];
        }

        private Dictionary<string, string> ConvertToDict(dynamic config)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(config))
            {
                dictionary.Add(propertyDescriptor.Name, Convert.ToString(propertyDescriptor.GetValue(config)));
            }
            return dictionary;
        }

        private IEnumerable<Position> ToLine(double[] pos)
        {
            for (int i = 0; i < pos.Length; i += 2)
            {
                var x = pos[i + 1];
                var y = pos[i];
                yield return new Position(x, y);
            }
        }

        private bool TacMapExists(int id)
        {
            return _context.TacMaps.Any(e => e.TacMapID == id);
        }


        // GET: TacMaps/Edit/5
        public async Task<IActionResult> Clone(int id)
        {
            var tacMap = await _mapSvc.GrantWriteAccess(User, id, null);
            if (tacMap == null)
            {
                return NotFound();
            }
            tacMap.TacMap.Label += " (copy)";
            return View(tacMap.TacMap);
        }

        // POST: TacMaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clone(int id, [Bind("WorldName,GameName,Label")] TacMap cloneInfos)
        {
            var tacMap = await _mapSvc.GrantWriteAccess(User, id, null);
            if (tacMap == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var clone = await _mapSvc.CreateMap(User, tacMap.TacMap.WorldName, tacMap.TacMap.GameName, cloneInfos.Label, null, tacMap.TacMap.FriendlyOrbatID, tacMap.TacMap.HostileOrbatID);

                var layerMapping = new Dictionary<int, int>
                {
                    { tacMap.TacMapID, clone.TacMapID }
                };

                foreach (var layer in await _mapSvc.GetLayers(tacMap.TacMapID))
                {
                    if (layer.Id != tacMap.TacMapID)
                    {
                        var clonedLayer = await _mapSvc.CreateLayer(User, clone, layer.Label, layer.Phase);
                        layerMapping.Add(layer.Id, clonedLayer.Id);
                    }
                }
                
                foreach (var marker in await _mapSvc.GetMarkers(tacMap.TacMapID, true))
                {
                    await _context.AddAsync(new TacMapMarker()
                    {
                        TacMapID = layerMapping[marker.LayerId],
                        UserID = tacMap.UserID,
                        MarkerData = marker.MarkerData,
                        LastUpdate = DateTime.UtcNow
                    });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id = clone.TacMapID });
            }
            return View(tacMap.TacMap);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportTexture(int id, int size, TextureFormat format)
        {
            if (!new[] {1024, 2048}.Contains(size))
            {
                return BadRequest();
            }
            var tacMap = await _mapSvc.GrantReadAccess(User, id, null);
            if (tacMap == null)
            {
                return NotFound();
            }
            var preview = await _preview.GetPreview(tacMap, 2048);

            using (var target = new Image<Rgba32>(size, size, format == TextureFormat.WhiteBoard ? new Rgba32(255, 255, 255) : new Rgba32(0, 0, 0)))
            {
                using (var source = Image.Load(preview))
                {
                    if (format == TextureFormat.WhiteBoard)
                    {
                        var sizeValue = (int)(size * 0.7d);
                        var position = (size - sizeValue) / 2;
                        source.Mutate(x => x.Resize(sizeValue, sizeValue));
                        target.Mutate(x => x.DrawImage(source, new SixLabors.ImageSharp.Point(position, position), 1.0f));
                    }
                    else
                    {
                        var sizeValue = size / 2;
                        var position = (size - sizeValue) / 2;
                        source.Mutate(x => x.Resize(sizeValue, size));
                        target.Mutate(x => x.DrawImage(source, new SixLabors.ImageSharp.Point(position, 0), 1.0f));
                    }
                }
                using (var ms = new MemoryStream())
                {
                    target.SaveAsJpeg(ms);
                    return File(ms.ToArray(), "image/jpeg", $"{id}-{size}-{format}.jpg");
                }
            }
        }
        
    }
}
