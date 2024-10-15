using System;
using Arma3TacMapWebApp.Services.GameMapStorage;
using Microsoft.AspNetCore.Mvc;

namespace Arma3TacMapWebApp.Controllers
{
    public class AtlasController : Controller
    {
        private IGameMapStorageService storageService;

        public AtlasController(IGameMapStorageService storageService) 
        {
            this.storageService = storageService;
        }

        [Route("Atlas")]
        public IActionResult Index()
        {
            return RedirectPermanent(new Uri(storageService.BaseUri, "/maps/arma3").AbsoluteUri);
        }

        [Route("Atlas/{id}")]
        public IActionResult Details(string id, double? x, double? y, string view)
        {
            if (x != null && y != null)
            {
                return RedirectPermanent(new Uri(storageService.BaseUri, FormattableString.Invariant($"/maps/arma3/{id}?x={x}&y={y}")).AbsoluteUri);
            }
            return RedirectPermanent(new Uri(storageService.BaseUri, $"/maps/arma3/{id}").AbsoluteUri);
        }
    }
}
