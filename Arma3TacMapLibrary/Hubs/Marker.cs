using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapLibrary.Hubs
{
    public class Marker<TMapId>
    {
        public TMapId mapId { get; set; }

        public int id { get; set; }

        public MarkerData data { get; set; }
    }
}
