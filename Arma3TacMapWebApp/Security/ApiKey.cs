using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using AspNetCore.Authentication.ApiKey;

namespace Arma3TacMapWebApp.Security
{
    public class ApiKey : IApiKey
    {
        public ApiKey(UserApiKey userApiKey, string key)
        {
            Key = key;
            OwnerName = userApiKey.User.UserLabel;
            Claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, $"User#{userApiKey.User.UserID}"),
                new Claim(ClaimTypes.Name, userApiKey.User.UserLabel),
                new Claim(User.UserIDClaim, userApiKey.User.UserID.ToString()),
                new Claim(User.IsServiceClaim, "true")
            };
        }

        public string Key { get; }

        public string OwnerName { get; }

        public IReadOnlyCollection<Claim> Claims { get; }
    }
}
