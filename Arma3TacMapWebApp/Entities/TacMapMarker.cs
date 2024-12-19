using System;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMapMarker
    {
        public int TacMapMarkerID { get; set; }

        public int TacMapID { get; set; }
        public TacMap? TacMap { get; set; }

        public int? UserID { get; set; }
        public User? User { get; set; }

        public string MarkerData { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
