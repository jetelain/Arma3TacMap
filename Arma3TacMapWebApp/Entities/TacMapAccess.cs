using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMapAccess
    {
        public int TacMapAccessID { get; set; }

        public int TacMapID { get; set; }
        public TacMap TacMap { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public bool CanWrite { get; set; }
    }
}
