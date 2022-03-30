using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Arma3TacMapLibrary;
using Arma3TacMapLibrary.Arma3;
using Arma3TacMapWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Arma3TacMapWebApp.Controllers
{
    public class AtlasController : Controller
    {
        private readonly MapInfosService _mapInfos;
        private readonly IAuthorizationService _authorizationService;
        private readonly IConfiguration _configuration;

        public AtlasController(MapInfosService mapInfos, IAuthorizationService authorizationService, IConfiguration configuration)
        {
            _mapInfos = mapInfos;
            _authorizationService = authorizationService;
            _configuration = configuration;
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
            ViewBag.IsFullScreen = false;
            ViewBag.HasSearchBox = true;
            return View(new AltasMapViewModel()
            {
                MapInfos = map,
                InitStaticMap = new StaticMapModel()
                {
                    center = (x != null && y != null) ? new double[] { y.Value, x.Value } : null,
                    endpoint = Arma3MapHelper.GetEndpoint(_configuration),
                    markers = new Dictionary<string, Arma3TacMapLibrary.Maps.MarkerData>(),
                    worldName = map.worldName
                }
            });
        }
    }
}
