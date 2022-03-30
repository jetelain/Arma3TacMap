using System.Collections.Generic;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class EditMapViewModel
    {
        public TacMapAccess Access { get; internal set; }

        public List<OrbatUnit> Friendly { get; internal set; }

        public List<OrbatUnit> Hostile { get; internal set; }

        public LiveMapModel InitLiveMap { get; set; }
    }
}
