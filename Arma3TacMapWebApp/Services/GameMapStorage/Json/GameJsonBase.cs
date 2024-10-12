using System;

namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GameJsonBase
    {
        public int GameId { get; set; }

        public string? EnglishTitle { get; set; }

        public string? Name { get; set; }

        public string? Attribution { get; set; }

        public string? OfficialSiteUri { get; set; }

        public string? SteamAppId { get; set; }

        public DateTime? LastChangeUtc { get; set; }

        public string? Logo { get; set; }

        public string? LogoWebp { get; set; }

        public string? LogoPng { get; set; }
    }
}
