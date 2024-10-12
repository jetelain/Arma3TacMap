using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class StaticMapData
    {
        public List<StoredMarker> Markers { get; set; }

        public string WorldName { get; set; }

        public double[] Center { get; set; }
        public string GameName { get; set; }
    }
}
