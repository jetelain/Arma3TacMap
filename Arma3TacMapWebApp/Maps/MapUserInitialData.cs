using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Maps
{
    public class MapUserInitialData
    {
        public bool CanRead { get; set; }

        public string PseudoUserId { get; set; }

        public List<StoredMarker> InitialMarkers { get; set; }

        public List<StoredLayer> InitialLayers { get; set; }

        public static MapUserInitialData Denied = new MapUserInitialData();
    }
}
