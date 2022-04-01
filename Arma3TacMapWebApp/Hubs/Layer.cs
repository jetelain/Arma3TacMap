using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Hubs
{
    public class Layer
    {
        public int id { get; set; }
        public LayerData data { get; set; }
        public MapId mapId { get; set; }
        public bool isDefaultLayer { get; set; }
    }
}