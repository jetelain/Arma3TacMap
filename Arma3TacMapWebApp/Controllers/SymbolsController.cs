using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Pmad.Milsymbol.App6d;
using Pmad.Milsymbol.AspNetCore.Services;
using Pmad.Milsymbol.Png;

namespace Arma3TacMapWebApp.Controllers
{
    public class SymbolsController : Controller
    {
        private readonly IApp6dSymbolGenerator _symbolGenerator;

        public SymbolsController(IApp6dSymbolGenerator symbolGenerator)
        {
            _symbolGenerator = symbolGenerator;
        }

        [Route("Symbols")]
        [Route("Symbols/{sidc:regex(^[[0-9]]{{20}}$)}")]
        public IActionResult Index(string? sidc = null, [FromQuery] SymbolsViewModel? options = null)
        {
            App6dSymbolIdInfos? infos = null;
            if (!string.IsNullOrEmpty(sidc) && App6dSymbolId.TryParse(sidc, out var id))
            {
                infos = App6dSymbolIdInfos.From(id);
            }
            return View(new SymbolsViewModel() { 
                Symbol = sidc,
                AdditionalInformation = options?.AdditionalInformation,
                CommonIdentifier = options?.CommonIdentifier,
                Direction = options?.Direction,
                HigherFormation = options?.HigherFormation,
                ReinforcedReduced = options?.ReinforcedReduced,
                UniqueDesignation = options?.UniqueDesignation,
                Infos = infos
            });
        }

        [Route("Symbols/png/{sidc:regex(^[[0-9]]{{20}}$)}")]
        public async Task<IActionResult> Png(string sidc, [FromQuery] SymbolsViewModel? options = null)
        {
            var symbol = await _symbolGenerator.GenerateAsync(sidc, new Pmad.Milsymbol.Icons.SymbolIconOptions()
            {
                AdditionalInformation = options?.AdditionalInformation,
                CommonIdentifier = options?.CommonIdentifier,
                Direction = ToDegrees(options?.Direction),
                HigherFormation = options?.HigherFormation,
                ReinforcedReduced = options?.ReinforcedReduced,
                UniqueDesignation = options?.UniqueDesignation
            });
            var stream = new MemoryStream();
            symbol.SaveToPng(stream, 2);
            stream.Position = 0;
            return File(stream, "image/png");
        }

        private double? ToDegrees(string? direction)
        {
            if (!string.IsNullOrEmpty(direction) && double.TryParse(direction, CultureInfo.InvariantCulture, out var mils))
            {
                return mils * 360 / 6400;
            }
            return null;
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
