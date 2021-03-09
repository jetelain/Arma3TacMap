using Arma3TacMapLibrary.Arma3;

namespace Arma3TacMapWebApp.Entities
{
    public class ReplayGroup
    {
        public int GroupNumber { get; set; }

        public int ReplayMapID { get; set; }

        public ReplayMap ReplayMap { get; set; }

        public Arma3Side? Side { get; set; }

        public string Name { get; set; }
    }
}
