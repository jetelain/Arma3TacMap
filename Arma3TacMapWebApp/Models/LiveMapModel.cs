using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class LiveMapModel : IMapCommonModel
    {
        public string hub { get; set; }

        public string endpoint { get; set; }

        public MapId mapId { get; set; }

        public string worldName { get; set; }

        public bool isReadOnly { get; set; }

        public string view { get; set; }

        string IMapCommonModel.init => "initLiveMap";
    }
}