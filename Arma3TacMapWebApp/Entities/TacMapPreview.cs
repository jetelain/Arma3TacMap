using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMapPreview
    {
        public int TacMapID { get; set; }

        public int Size { get; set; }

        public byte[] Data { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
