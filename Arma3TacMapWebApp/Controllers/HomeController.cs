using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Arma3TacMapWebApp.Models;
using Arma3TacMapWebApp.Maps;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Arma3TacMapWebApp.Entities;

namespace Arma3TacMapWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MapInfosService _mapInfos;
        private readonly MapService _mapSvc;
        public HomeController(ILogger<HomeController> logger, MapInfosService mapInfos, MapService mapSvc)
        {
            _logger = logger;
            _mapInfos = mapInfos;
            _mapSvc = mapSvc;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new IndexViewModel();
            vm.Maps = await _mapInfos.GetMapsInfos();
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
                MapId = new MapId()
                {
                    TacMapID = access.TacMap.TacMapID,
                    IsReadOnly = false,
                    ReadToken = null
                },
                Access = access
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
            return View(new EditMapViewModel()
            {
                MapId = new MapId()
                {
                    TacMapID = access.TacMap.TacMapID,
                    IsReadOnly = true,
                    ReadToken = t
                },
                Access = access
            });
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
