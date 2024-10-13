using System.Collections.Generic;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;

namespace Arma3TacMapWebApp.Models
{
    public class IndexViewModel
    {
        public required List<TacMapAccess> TacMaps { get; set; }
        public required List<GameJsonBase> Games { get; set; }
    }
}
