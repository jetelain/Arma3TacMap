using System.Collections.Generic;
using System.Linq;
using Arma3TacMapLibrary.Arma3;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class AltasMapViewModel
    {
        public MapInfos MapInfos { get; internal set; }

        public StaticMapModel InitStaticMap { get; set; }

        public bool HasTopo => new[] { "xcam_taunus", "gossi", "gtd_taunus", "altis" }.Contains(InitStaticMap.worldName);
    }
}
