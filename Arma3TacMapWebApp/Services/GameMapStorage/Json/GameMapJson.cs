using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GameMapJson : GameMapJsonBase
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Attribution { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GameMapLocationJson>? Locations { get; set; }
    }
}
