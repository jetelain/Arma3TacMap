using System.Collections.Generic;

namespace Arma3TacMapWebApp.Entities
{
    public class User
    {
        internal static string UserIDClaim = "Arma3TacMapWebApp.Entities.User.UserID";
        internal static string IsServiceClaim = "Arma3TacMapWebApp.Entities.User.IsService";

        public int UserID { get; set; }

        public string UserLabel { get; set; }

        public string SteamId { get; set; }

        public bool IsService { get; set; }

        public List<UserApiKey> ApiKeys { get; set; }
    }
}
