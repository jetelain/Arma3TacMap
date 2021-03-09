using System.Collections.Generic;
using System.Text.Json;

namespace Arma3TacMapWebApp.Services
{
    public class AarObjects
    {
        public List<List<JsonElement>> units { get; set; }
        public List<List<JsonElement>> vehs { get; set; }
    }

    public class AarMetadata
    {
        public string island { get; set; }
        public string name { get; set; }
        public int time { get; set; }
        public string date { get; set; }
        public string desc { get; set; }
        public List<List<string>> players { get; set; }
        public AarObjects objects { get; set; }
    }

    public class AarJson
    {
        public AarMetadata metadata { get; set; }
        public List<List<List<List<int>>>> timeline { get; set; }
    }
}
