using System;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GameMarkerJson
    {
        public int GameMarkerId { get; set; }

        public string? EnglishTitle { get; set; }

        public string? Name { get; set; }

        public MarkerUsage Usage { get; set; }

        public string? ImagePng { get; set; }

        public string? ImageWebp { get; set; }

        public bool IsColorCompatible { get; set; }

        public DateTime? ImageLastChangeUtc { get; set; }

        public string? MilSymbolEquivalent { get; set; }

        public string? SteamWorkshopId { get; set; }
    }
}