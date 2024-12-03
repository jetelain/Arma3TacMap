using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Arma3TacMapWebApp.Entities
{
    public class UserApiKey
    {
        public int UserApiKeyID { get; set; }

        public string HashedKey { get; set; }

        public string Salt { get; set; }

        public DateTime? ValidUntil { get; set; }

        public int UserID { get; set; }

        public User? User { get; set; }

        internal bool IsValidToken(string token)
        {
            var salt = Convert.FromBase64String(Salt);
            return HashedKey == HashToken(token, salt);
        }

        private static string HashToken(string key, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(key, salt, KeyDerivationPrf.HMACSHA512, 10000, 32));
        }

        internal void SetToken(string key)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            Salt = Convert.ToBase64String(salt);
            HashedKey = HashToken(key, salt);
        }

        public string MaskedKey
        {
            get { return $"{UserApiKeyID:X}:*********************************************"; }
        }

        public string ToKey(string token)
        {
            return $"{UserApiKeyID:X}:{token}";
        }
    }
}
