using System;

namespace Arma3TacMapLibrary.Maps
{
    public class StoredMarker
    {
        public int Id { get; set; }

        public int LayerId { get; set; }

        public string MarkerData { get; set; }

        public bool IsReadOnly { get; set; }
    }
}