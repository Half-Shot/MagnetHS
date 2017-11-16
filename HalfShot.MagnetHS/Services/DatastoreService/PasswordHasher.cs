using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace HalfShot.MagnetHS.DatastoreService
{
    public class PasswordHasher
    {
        public const int IterationCount = 10000;
        public const int KeyLength = 256;
        public const int SaltLength = 32;
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
    }
}
