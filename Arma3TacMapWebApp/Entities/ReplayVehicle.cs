namespace Arma3TacMapWebApp.Entities
{
    public class ReplayVehicle
    {
        public int VehicleNumber { get; set; }

        public int ReplayMapID { get; set; }

        public ReplayMap ReplayMap { get; set; }

        public string ClassName { get; set; }
    }
}
