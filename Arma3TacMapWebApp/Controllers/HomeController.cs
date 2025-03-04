using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Models;
using Arma3TacMapWebApp.Services.GameMapStorage;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Arma3TacMapWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameMapStorageService _mapInfos;
        private readonly MapService _mapSvc;
        private readonly IMapPreviewService _preview;
        private readonly IAuthorizationService _authorizationService;

        public HomeController(ILogger<HomeController> logger, IGameMapStorageService mapInfos, MapService mapSvc, IMapPreviewService preview, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _mapInfos = mapInfos;
            _mapSvc = mapSvc;
            _preview = preview;
            _authorizationService = authorizationService;
        }

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
                TacMaps = await _mapSvc.GetUserMaps(User, 6)
            };
            foreach(var map in vm.TacMaps)
            {
                map.TacMap.MapInfos = await _mapInfos.GetMapBase(map.TacMap.GameName, map.TacMap.WorldName);
            }
            return View(vm);
        }

        [Authorize(Policy = "LoggedUser")]
        [HttpGet]
        public IActionResult CreateMap(string id)
        {
            return RedirectToAction(nameof(TacMapsController.Create), "TacMaps", new { worldName = id });
        }
        /*
        [Authorize(Policy = "LoggedUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMap(string id, string label)
        {
            MapId mapId = await _mapSvc.CreateMap(User, id, label ?? "Carte tactique sans nom");
            return RedirectToAction(nameof(EditMap), new { id = mapId.TacMapID });
        }
        */

        [Route("EditMap/{id}")]
        [Authorize(Policy = "LoggedUser")]
        public async Task<IActionResult> EditMap(int id, string t, string view)
        {
            var access = await _mapSvc.GrantWriteAccess(User, id, t);
            if (access == null)
            {
                return Forbid();
            }

            var game = await _mapInfos.GetGame(access.TacMap.GameName) ?? throw new ApplicationException("Unknown game");
            var gameMap = await _mapInfos.GetMap(access.TacMap.GameName, access.TacMap.WorldName) ?? throw new ApplicationException("Unknown map");

            return View(new EditMapViewModel()
            {
                InitLiveMap = new LiveMapModel()
                {
                    endpoint = _mapInfos.LegacyEndpoint.AbsoluteUri,
                    hub = "/MapHub",
                    isReadOnly = false,
                    mapId = new MapId()
                    {
                        TacMapID = access.TacMap.TacMapID,
                        IsReadOnly = false,
                        ReadToken = null
                    },
                    worldName = access.TacMap.WorldName,
                    view = view,
                    Game = game,
                    GameMap = gameMap,
                    GmsBaseUri = _mapInfos.BaseUri.AbsoluteUri
                },
                Access = access,
                Friendly = await _mapSvc.GetOrbatUnits(access.TacMap.FriendlyOrbatID),
                Hostile = await _mapSvc.GetOrbatUnits(access.TacMap.HostileOrbatID)
            });
        }

        [Route("ViewMap/{id}")]
        public async Task<IActionResult> ViewMap(int id, string? t = null, string? view = null)
        {
            var access = await _mapSvc.GrantReadAccess(User, id, t);
            if (access == null)
            {
                return Forbid();
            }
            var game = await _mapInfos.GetGame(access.TacMap.GameName) ?? throw new ApplicationException("Unknown game");
            var gameMap = await _mapInfos.GetMap(access.TacMap.GameName, access.TacMap.WorldName) ?? throw new ApplicationException("Unknown map");

            ViewBag.IsFullScreen = false;
            return View(new EditMapViewModel()
            {
                Access = access,
                InitLiveMap = new LiveMapModel()
                {
                    endpoint = _mapInfos.LegacyEndpoint.AbsoluteUri,
                    hub = "/MapHub",
                    isReadOnly = true,
                    mapId = new MapId()
                    {
                        TacMapID = access.TacMap.TacMapID,
                        IsReadOnly = true,
                        ReadToken = t
                    },
                    worldName = access.TacMap.WorldName,
                    view = view,
                    Game = game,
                    GameMap = gameMap,
                    GmsBaseUri = _mapInfos.BaseUri.AbsoluteUri
                }
            });
        }

        [Route("ViewMap/{id}/FullScreen")]
        public async Task<IActionResult> ViewMapFullStatic(int id, string t, int? phase = null)
        {
            var data = await _mapSvc.GetStaticMapModel(id, t, phase);
            if (data == null)
            {
                return Forbid();
            }
            var game = await _mapInfos.GetGame(data.GameName) ?? throw new ApplicationException("Unknown game");
            var gameMap = await _mapInfos.GetMap(data.GameName, data.WorldName) ?? throw new ApplicationException("Unknown map");
            ViewBag.IsFullScreen = true;
            return View(new StaticMapModel(){
                center = data.Center,
                GmsBaseUri = _mapInfos.BaseUri.AbsolutePath,
                endpoint = _mapInfos.LegacyEndpoint.AbsoluteUri,
                markers = data.Markers.ToDictionary(m => m.Id.ToString(), m => MarkerData.Deserialize(m.MarkerData)),
                worldName = data.WorldName,
                fullScreen = true,
                GameMap=gameMap,
                Game=game
            });
        }

        [Route("ViewMap/{id}/LiveFullScreen")]
        public async Task<IActionResult> ViewMapFullLive(int id, string t)
        {
            var result = await ViewMap(id, t, null);
            ViewBag.IsFullScreen = true;
            return result;
        }

        [Route("ViewMap/{id}/Preview/{size=512}")]
        public async Task<IActionResult> ViewMapScreenShot(int id, int size, string t, int? phase = null)
        {
            var access = await _mapSvc.GrantReadAccess(User, id, t);
            if (access == null)
            {
                return Forbid();
            }
            if (!_preview.ValidSizes.Contains(size))
            {
                return NotFound();
            }
            return File(await _preview.GetPreview(access, size, phase), size > 512 ? "image/jpeg" : "image/png");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Route("css/game-{gameName}.css")]
        public async Task<IActionResult> GameCssContent(string gameName)
        {
            var game = await _mapInfos.GetGame(gameName);
            if (game == null)
            {
                return NotFound();
            }
            var sb = new StringBuilder();
            if (game.Colors != null)
            {
                foreach (var color in game.Colors)
                {
                    sb.Append($".game-bg-{color.Name!.ToLowerInvariant()} {{ color: {color.ContrastHexadecimal} !important; background-color: {color.Hexadecimal} !important; }}");
                    sb.AppendLine();
                }
            }
            if ( game.Markers != null)
            {
                foreach (var marker in game.Markers)
                {
                    sb.Append($".game-icon-{marker.Name!.ToLowerInvariant()} {{ background-image: url('{marker.ImagePng}'); }}");
                    sb.AppendLine();
                }
            }
            return Content(sb.ToString(), "text/css");
        }

    }
}
