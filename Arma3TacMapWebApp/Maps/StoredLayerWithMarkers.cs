using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Maps
{
    public class StoredLayerWithMarkers : StoredLayer
    {
        public List<StoredMarker> Markers { get; set; }
    }
}