using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Entities
{
    public class OrbatAccess
    {
        public int OrbatAccessID { get; set; }

        public int OrbatID { get; set; }
        public Orbat Orbat { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }
}
