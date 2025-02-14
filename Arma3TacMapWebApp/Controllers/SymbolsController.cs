using System;
using Arma3TacMapWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Pmad.Milsymbol.App6d;

namespace Arma3TacMapWebApp.Controllers
{
    public class SymbolsController : Controller
    {
        [Route("Symbols")]
        [Route("Symbols/{sidc:regex(^[[0-9]]{{20}}$)}")]
        public IActionResult Index(string? sidc = null, [FromQuery] SymbolsViewModel? options = null)
        {
            return View(new SymbolsViewModel() { 
                Symbol = sidc,
                AdditionalInformation = options?.AdditionalInformation,
                CommonIdentifier = options?.CommonIdentifier,
                Direction = options?.Direction,
                HigherFormation = options?.HigherFormation,
                ReinforcedReduced = options?.ReinforcedReduced,
                UniqueDesignation = options?.UniqueDesignation
            });
        }

        //[Route("Symbols/Bookmarks")]
        //public IActionResult Bookmarks()
        //{
        //    return View();
        //}

        [Route("Symbols/All")]
        public IActionResult All()
        {
            return View(App6dSymbolDatabase.Default);
        }
    }
}
