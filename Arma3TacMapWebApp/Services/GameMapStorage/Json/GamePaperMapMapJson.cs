namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GamePaperMapMapJson : GamePaperMapJson
    {
        public int GameMapId { get; set; }

        public string? EnglishTitle { get; set; }

        public string? AppendAttribution { get; set; }

        public string? MapName { get; set; }

    }
}
