using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Arma3;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly MapInfosService _mapInfos;
        private readonly MapService _mapSvc;
        private readonly MapPreviewService _preview;
        private readonly IAuthorizationService _authorizationService;

        public TacMapsController(Arma3TacMapContext context, MapInfosService mapInfos, MapService mapSvc, MapPreviewService preview, IAuthorizationService authorizationService)
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
            var maps = await _mapInfos.GetMapsInfosFilter((await _authorizationService.AuthorizeAsync(User, "WorkInProgress")).Succeeded);
            var list = await _mapSvc.GetUserMaps(User);
            foreach (var map in list)
            {
                map.TacMap.MapInfos = maps.FirstOrDefault(m => m.worldName == map.TacMap.WorldName);
            }
            return View(list);
        }

        // GET: TacMaps/Create
        public async Task<IActionResult> Create(string worldName)
        {
            var user = await _mapSvc.GetUser(User);
            await PrepareOrbatDropdown(user);
            ViewBag.Maps = await _mapInfos.GetMapsInfosFilter((await _authorizationService.AuthorizeAsync(User, "WorkInProgress")).Succeeded); 
            return View(new TacMap()
            {
                WorldName = worldName,
                Label = "Carte tactique sans nom"
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
        public async Task<IActionResult> Create([Bind("Label,WorldName,FriendlyOrbatID,HostileOrbatID")] TacMap tacMap)
        {
            if (ModelState.IsValid)
            {
                var map = await _mapSvc.CreateMap(User, tacMap.WorldName, tacMap.Label, null, tacMap.FriendlyOrbatID, tacMap.HostileOrbatID);
                return RedirectToAction(nameof(HomeController.EditMap), "Home", new { id = map.TacMapID });
            }
            ViewBag.Maps = await _mapInfos.GetMapsInfos();
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
            if (tacMap == null)
            {
                return NotFound();
            }
            var user = await _mapSvc.GetUser(User);
            if (tacMap.OwnerUserID != user.UserID)
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
        public async Task<IActionResult> Edit(int id, [Bind("TacMapID,Label,WorldName,FriendlyOrbatID,HostileOrbatID")] TacMap edited)
        {
            if (id != edited.TacMapID)
            {
                return NotFound();
            }
            var tacMap = await _context.TacMaps
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.TacMapID == id);

            var user = await _mapSvc.GetUser(User);
            if (tacMap.OwnerUserID != user.UserID)
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
            if (tacMap == null)
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
            if (tacMap.OwnerUserID != (await _mapSvc.GetUser(User)).UserID)
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


        public async Task<IActionResult> Export(int id)
        {
            var mapAccess = await _mapSvc.GrantWriteAccess(User, id, null);
            if (mapAccess == null)
            {
                return Forbid();
            }

            return View(new ExportViewModel()
            {
                TacMap = mapAccess.TacMap,
                Access = mapAccess,
                Script = MapExporter.GetSqf(await _mapSvc.GetMarkers(id, true))
            });
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
        public async Task<IActionResult> Clone(int id, [Bind("WorldName,Label")] TacMap cloneInfos)
        {
            var tacMap = await _mapSvc.GrantWriteAccess(User, id, null);
            if (tacMap == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var clone = await _mapSvc.CreateMap(User, tacMap.TacMap.WorldName, cloneInfos.Label, null, tacMap.TacMap.FriendlyOrbatID, tacMap.TacMap.HostileOrbatID);
                foreach(var marker in await _mapSvc.GetMarkers(tacMap.TacMapID, true))
                {
                    await _context.AddAsync(new TacMapMarker()
                    {
                        TacMapID = clone.TacMapID,
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
