using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GameJson : GameJsonBase
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GameColorJson>? Colors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GameMarkerJson>? Markers { get; set; }
    }
}
