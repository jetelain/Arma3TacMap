using System.Collections.Generic;
using System.Text.Json;
using Ganss.Xss;

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
            if (data.type == "note")
            {
                if (data.config.TryGetValue("content", out var content))
                {
                    if (content.Length > 50000)
                    {
                        content = content.Substring(0, 50000);
                    }
                    var san = new HtmlSanitizer();
                    data.config["content"] = san.Sanitize(content);
                }
                data.config.Remove("html");
            }
            if (data.scale == 0)
            {
                data.scale = null;
            }
            return JsonSerializer.Serialize<MarkerData>(data, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        public static MarkerData Deserialize(string data)
        {
            var markerdata = JsonSerializer.Deserialize<MarkerData>(data, new JsonSerializerOptions() { IgnoreNullValues = true });
            if (markerdata.scale == 0)
            {
                markerdata.scale = null;
            }
            return markerdata;
        }
    }
}
