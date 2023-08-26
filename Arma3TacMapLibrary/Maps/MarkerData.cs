using System.Collections.Generic;
using System.Text.Json;

namespace Arma3TacMapLibrary.Maps
{
    public class MarkerData
    {
        public string type { get; set; }

        public string symbol { get; set; }

        public Dictionary<string,string> config { get; set; }

        public double[] pos { get; set; }

        public double? scale { get; set; }

        public static string Serialize(MarkerData data)
        {
            return JsonSerializer.Serialize<MarkerData>(data, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        public static MarkerData Deserialize(string data)
        {
            return JsonSerializer.Deserialize<MarkerData>(data, new JsonSerializerOptions() { IgnoreNullValues = true });
        }
    }
}
