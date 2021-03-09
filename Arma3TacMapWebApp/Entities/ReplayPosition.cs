using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Entities
{
    public class ReplayPosition
    {
        [JsonPropertyName("a")]
        public bool IsAlive { get; set; }

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("d")]
        public double Direction { get; set; }

        [JsonPropertyName("p")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PlayerNumber { get; set; }

        [JsonPropertyName("u")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? UnitNumber { get; set; }

        [JsonPropertyName("v")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? VehicleNumber { get; set; }

        [JsonPropertyName("g")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? GroupNumber { get; set; }
    }
}