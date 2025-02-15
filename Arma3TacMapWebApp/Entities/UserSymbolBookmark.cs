namespace Arma3TacMapWebApp.Entities
{
    public class UserSymbolBookmark
    {
        public int UserSymbolBookmarkID { get; set; }

        public int UserID { get; set; }
        public User? User { get; set; }

        public required string Sidc { get; set; }
        public string? Label { get; set; }
    }
}
