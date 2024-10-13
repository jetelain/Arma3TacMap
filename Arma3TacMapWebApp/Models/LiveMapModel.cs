using Arma3TacMapLibrary.Maps;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;

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

        public required GameJson Game { get; set; }

        public required GameMapJson GameMap { get; set; }

        public required string GmsBaseUri { get; set; }

        string IMapCommonModel.init => "initLiveMap";
    }
}