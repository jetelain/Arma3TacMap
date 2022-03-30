using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapLibrary;
using Arma3TacMapLibrary.Arma3;
using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;
using Arma3TacMapWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Arma3TacMapWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MapInfosService _mapInfos;
        private readonly MapService _mapSvc;
        private readonly MapPreviewService _preview;
        private readonly IAuthorizationService _authorizationService;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, MapInfosService mapInfos, MapService mapSvc, MapPreviewService preview, IAuthorizationService authorizationService, IConfiguration configuration)
        {
            _logger = logger;
            _mapInfos = mapInfos;
            _mapSvc = mapSvc;
            _preview = preview;
            _authorizationService = authorizationService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new IndexViewModel();
            vm.Maps = await _mapInfos.GetMapsInfosFilter((await _authorizationService.AuthorizeAsync(User, "WorkInProgress")).Succeeded);
            vm.TacMaps = await _mapSvc.GetUserMaps(User, 6);
            foreach(var map in vm.TacMaps)
            {
                map.TacMap.MapInfos = vm.Maps.FirstOrDefault(m => m.worldName == map.TacMap.WorldName);
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
        public async Task<IActionResult> EditMap(int id, string t)
        {
            TacMapAccess access = await _mapSvc.GrantWriteAccess(User, id, t); 
            if (access == null)
            {
                return Forbid();
            }
            return View(new EditMapViewModel()
            {
                InitLiveMap = new LiveMapModel()
                {
                    endpoint = Arma3MapHelper.GetEndpoint(_configuration),
                    hub = "/MapHub",
                    isReadOnly = false,
                    mapId = new MapId()
                    {
                        TacMapID = access.TacMap.TacMapID,
                        IsReadOnly = false,
                        ReadToken = null
                    },
                    worldName = access.TacMap.WorldName
                },
                Access = access,
                Friendly = await _mapSvc.GetOrbatUnits(access.TacMap.FriendlyOrbatID),
                Hostile = await _mapSvc.GetOrbatUnits(access.TacMap.HostileOrbatID),
            });
        }

        [Route("ViewMap/{id}")]
        public async Task<IActionResult> ViewMap(int id, string t)
        {
            var access = await _mapSvc.GrantReadAccess(User, id, t);
            if (access == null)
            {
                return Forbid();
            }
            ViewBag.IsFullScreen = false;
            return View(new EditMapViewModel()
            {
                Access = access,
                InitLiveMap = new LiveMapModel()
                {
                    endpoint = Arma3MapHelper.GetEndpoint(_configuration),
                    hub = "/MapHub",
                    isReadOnly = true,
                    mapId = new MapId()
                    {
                        TacMapID = access.TacMap.TacMapID,
                        IsReadOnly = true,
                        ReadToken = t
                    },
                    worldName = access.TacMap.WorldName
                }
            });
        }

        [Route("ViewMap/{id}/FullScreen")]
        public async Task<IActionResult> ViewMapFullStatic(int id, string t)
        {
            var data = await _mapSvc.GetStaticMapModel(id, t);
            if (data == null)
            {
                return Forbid();
            }
            ViewBag.IsFullScreen = true;
            return View(new StaticMapModel(){
                center = data.Center,
                endpoint = Arma3MapHelper.GetEndpoint(_configuration),
                markers = data.Markers.ToDictionary(m => m.Id.ToString(), m => MarkerData.Deserialize(m.MarkerData)),
                worldName = data.WorldName
            });
        }

        [Route("ViewMap/{id}/LiveFullScreen")]
        public async Task<IActionResult> ViewMapFullLive(int id, string t)
        {
            var result = await ViewMap(id, t);
            ViewBag.IsFullScreen = true;
            return result;
        }

        [Route("ViewMap/{id}/Preview/{size=512}")]
        public async Task<IActionResult> ViewMapScreenShot(int id, int size, string t)
        {
            var access = await _mapSvc.GrantReadAccess(User, id, t);
            if (access == null)
            {
                return Forbid();
            }
            if (!MapPreviewService.ValidSizes.Contains(size))
            {
                return NotFound();
            }
            return File(await _preview.GetPreview(access, size), size > 512 ? "image/jpeg" : "image/png");
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
    }
}
