using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Entities
{
    public class User
    {
        public int UserID { get; set; }

        public string UserLabel { get; set; }

        public string SteamId { get; set; }
    }
}
