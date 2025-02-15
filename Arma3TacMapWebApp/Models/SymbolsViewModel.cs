using Pmad.Milsymbol.App6d;

namespace Arma3TacMapWebApp.Models
{
    public class SymbolsViewModel
    {
        public string? Symbol { get; set; }
        public string? CommonIdentifier { get; set; }
        public string? HigherFormation { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? UniqueDesignation { get; set; }
        public string? Direction { get; set; }
        public string? ReinforcedReduced { get; set; }
        public App6dSymbolIdInfos? Infos { get; set; }
    }
}
