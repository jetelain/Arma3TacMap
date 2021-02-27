using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapLibrary.TacMaps
{
    public class ApiTacMapCreate : ApiTacMapPatch
    {
        public string WorldName { get; set; }
        public List<StoredMarker> Markers { get; set; }
    }
}