using System.Collections.Generic;
using Arma3TacMapLibrary.Maps;

namespace Arma3TacMapLibrary.TacMaps
{
    public class ApiTacMap : ApiTacMapCreate
    {
        public int Id { get; set; }
        public string ReadOnlyToken { get; set; }
        public string ReadOnlyHref { get; set; }
        public string ReadWriteToken { get; set; }
        public string ReadWriteHref { get; set; }
        public Dictionary<int,string> PreviewHref { get; set; }

        public MapId ToMapId()
        {
            return new MapId() { IsReadOnly = true, ReadToken = ReadOnlyToken, TacMapID = Id };
        }
    }
}