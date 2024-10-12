using System.Collections.Generic;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;

namespace Arma3TacMapWebApp.Models
{
    public class IndexViewModel
    {
        //public List<MapInfos> Maps { get; set; }
        public List<TacMapAccess> TacMaps { get; internal set; }
        public List<GameJsonBase> Games { get; internal set; }
    }
}
