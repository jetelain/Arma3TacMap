using Arma3TacMapWebApp.Entities;
using Arma3TacMapWebApp.Maps;

namespace Arma3TacMapWebApp.Models
{
    public class EditMapViewModel
    {
        public MapId MapId { get; set; }
        public TacMapAccess Access { get; internal set; }
    }
}
