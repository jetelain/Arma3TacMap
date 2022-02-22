using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Arma3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arma3TacMapWebApp.Controllers
{
    public class AtlasController : Controller
    {
        private readonly MapInfosService _mapInfos;
        private readonly IAuthorizationService _authorizationService;

        public AtlasController(MapInfosService mapInfos, IAuthorizationService authorizationService)
        {
            _mapInfos = mapInfos;
            _authorizationService = authorizationService;
        }

        [Route("Atlas")]
        public async Task<IActionResult> Index()
        {
            return View(await _mapInfos.GetMapsInfosFilter((await _authorizationService.AuthorizeAsync(User, "WorkInProgress")).Succeeded));
        }

        [Route("Atlas/{id}")]
        public async Task<IActionResult> Details(string id, double? x, double? y)
        {
            var map = (await _mapInfos.GetMapsInfos()).FirstOrDefault(map => map.worldName == id);

            if (x != null && y != null)
            {
                ViewBag.Center = new double[] { y.Value, x.Value };
            }
            else
            {
                ViewBag.Center = null;
            }

            return View(map);
        }
    }
}
