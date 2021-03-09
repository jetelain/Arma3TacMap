namespace Arma3TacMapWebApp.Entities
{
    public class ReplayPlayer
    {
        public int PlayerNumber { get; set; }

        public int ReplayMapID { get; set; }

        public ReplayMap ReplayMap { get; set; }

        public string Uid { get; set; } // ?

        public string Name { get; set; }
    }
}
