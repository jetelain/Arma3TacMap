using System;
using System.Collections.Generic;
using System.Text;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapLibrary.TacMaps
{
    public class MapId : IMapId
    {
        public int TacMapID { get; set; }

        public string ReadToken { get; set; }

        public bool IsReadOnly { get; set; }

        public string GetGroup()
        {
            return $"TacMap-{TacMapID}";
        }
    }
}
