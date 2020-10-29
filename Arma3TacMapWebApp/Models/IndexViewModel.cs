using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class IndexViewModel
    {
        public List<MapInfos> Maps { get; set; }
        public List<TacMapAccess> TacMaps { get; internal set; }
    }
}
