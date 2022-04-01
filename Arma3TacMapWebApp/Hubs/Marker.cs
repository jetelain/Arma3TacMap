using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Hubs
{
    public class Marker
    {
        public MapId mapId { get; set; }

        public int id { get; set; }

        public MarkerData data { get; set; }

        public int layerId { get; set; }
    }
}
