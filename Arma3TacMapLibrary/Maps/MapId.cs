using System;
using System.Collections.Generic;
using System.Text;
using Arma3TacMapLibrary.Maps;


namespace Arma3TacMapLibrary.Maps
{
    public class MapId
    {
        public int TacMapID { get; set; }

        public string ReadToken { get; set; }

        public bool IsReadOnly { get; set; }
    }
}
