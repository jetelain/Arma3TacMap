using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Arma3TacMapWebApp.Entities
{
    public class ReplayFrameData
    {
        [JsonPropertyName("p")]
        public List<ReplayPosition> Positions { get; set; }
    }
}