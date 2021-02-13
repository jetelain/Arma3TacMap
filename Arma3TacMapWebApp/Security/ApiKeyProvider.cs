using System.Threading.Tasks;
using Arma3TacMapWebApp.Entities;
using AspNetCore.Authentication.ApiKey;
using Microsoft.EntityFrameworkCore;

namespace Arma3TacMapWebApp.Security
{
    public class ApiKeyProvider : IApiKeyProvider
    {
        private readonly Arma3TacMapContext _db;

        public ApiKeyProvider(Arma3TacMapContext db)
        {
            _db = db;
        }

        public async Task<IApiKey> ProvideAsync(string key)
        {
            var i = key.IndexOf(':');
            if ( i == -1 )
            {
                return null;
            }
            var keyId = int.Parse(key.Substring(0, i), System.Globalization.NumberStyles.HexNumber);
            var userKey = await _db.UserApiKeys.Include(k => k.User).FirstOrDefaultAsync(k => k.UserApiKeyID == keyId);
            if (userKey != null && userKey.IsValidToken(key.Substring(i + 1)))
            {
                return new ApiKey(userKey, key);
            }
            return null;
        }
    }
}
