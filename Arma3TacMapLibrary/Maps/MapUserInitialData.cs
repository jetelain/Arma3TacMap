using System.Collections.Generic;

namespace Arma3TacMapLibrary.Maps
{
    public class MapUserInitialData
    {
        public bool CanRead { get; set; }

        public string PseudoUserId { get; set; }

        public List<StoredMarker> InitialMarkers { get; set; }

        public static MapUserInitialData Denied = new MapUserInitialData();
    }
}
