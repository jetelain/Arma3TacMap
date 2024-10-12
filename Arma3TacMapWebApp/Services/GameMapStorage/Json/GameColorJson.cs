namespace Arma3TacMapWebApp.Services.GameMapStorage.Json
{
    public class GameColorJson
    {
        public int GameColorId { get; set; }

        public string? EnglishTitle { get; set; }

        public string? Name { get; set; }

        public string? Hexadecimal { get; set; }

        public ColorUsage Usage { get; set; }

        public string? ContrastHexadecimal { get; set; }
    }
}