using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMap
    {
        public int TacMapID { get; set; }

        public int OwnerUserID { get; set; }
        public User Owner { get; set; }

        public DateTime Created { get; set; }

        public string Label { get; set; }

        public string ReadOnlyToken { get; set; }

        public string ReadWriteToken { get; set; }

        public string WorldName { get; set; }
    }
}
