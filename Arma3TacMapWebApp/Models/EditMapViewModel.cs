using System.Collections.Generic;
using System.Linq;
using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Services.GameMapStorage.Json;

namespace Arma3TacMapWebApp.Models
{
    public class EditMapViewModel
    {
        public required TacMapAccess Access { get; set; }

        public List<OrbatUnit>? Friendly { get; internal set; }

        public List<OrbatUnit>? Hostile { get; internal set; }

        public required LiveMapModel InitLiveMap { get; set; }

        public bool HasTopo => new[] { "xcam_taunus", "gossi", "gtd_taunus", "altis" }.Contains(Access.TacMap.WorldName);

        public GameJson Game => InitLiveMap.Game;
    }
}
