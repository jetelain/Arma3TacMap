using Arma3TacMapLibrary.Arma3;

namespace Arma3TacMapWebApp.Entities
{
    public class ReplayUnit
    {
        public int UnitNumber { get; set; }

        public int ReplayMapID { get; set; }

        public ReplayMap ReplayMap { get; set; }

        public string ClassName { get; set; }

        public string Name { get; set; }

        public Arma3Side? Side { get; set; }
    }
}
