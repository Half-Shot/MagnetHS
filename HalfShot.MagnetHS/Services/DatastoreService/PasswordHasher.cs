using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace HalfShot.MagnetHS.DatastoreService
{
    public class HashingProvider
    {
        public const int IterationCount = 10000;
        public const int KeyLength = 256;
        public const int AccessTokenLength = 128;
        public const int SaltLength = 32;

        const int ASCII_MAX = 122;
        const int ASCII_MIN = 48;

        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        public static byte[] HashPassword(string password, byte[] salt)
        {
            Rfc2898DeriveBytes hasher = new Rfc2898DeriveBytes(password, salt, IterationCount);
            return hasher.GetBytes(KeyLength);
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltLength];
            rngCsp.GetBytes(salt);
            return salt;
        }

        public static string GenerateAccessToken()
        {
            List<byte> accessToken = new List<byte>();
            while(accessToken.Count < AccessTokenLength)
            {
                byte[] access_token = new byte[AccessTokenLength-accessToken.Count];
                rngCsp.GetNonZeroBytes(access_token);
                accessToken.AddRange(
                    access_token.Where((b) => (b <= ASCII_MAX && b >= ASCII_MIN) && Char.IsLetterOrDigit((char)b))
                );
            }
            return Encoding.ASCII.GetString(accessToken.ToArray());
        }
    }
}
