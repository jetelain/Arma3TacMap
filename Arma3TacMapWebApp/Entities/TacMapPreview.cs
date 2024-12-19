using System;

namespace Arma3TacMapWebApp.Entities
{
    public class TacMapPreview
    {
        public int TacMapID { get; set; }

        public int Size { get; set; }

        public required string PhaseKey { get; set; }

        public required byte[] Data { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}
