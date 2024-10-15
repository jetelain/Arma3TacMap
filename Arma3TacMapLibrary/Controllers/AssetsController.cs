using System;
using Microsoft.AspNetCore.Mvc;

namespace Arma3TacMapLibrary.Controllers
{
    public class AssetsController : Controller
    {
        [Obsolete]
        [HttpGet("/img/markers/{color}/{marker}.png")]
        [ResponseCache(Duration = 1440)]
        public IActionResult Arma3Marker(string color, string marker)
        {
            return RedirectPermanent($"https://atlas.plan-ops.fr/data/1/markers/{color}/{marker}.png");
        }
    }
}
