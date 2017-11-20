using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.DatastoreService;

namespace HalfShot.MagnetHS.Tests.Services.DatastoreService
{
    [TestClass]
    public class TestHashingProvider
    {
        [TestMethod]
        public void TestHashingProviderGenerateSalt()
        {
            byte[] result = HashingProvider.GenerateSalt();
            Assert.AreEqual(HashingProvider.SaltLength, result.Length);
        }

        [TestMethod]
        public void TestHashingProviderHashPassword()
        {
            byte[] salt = new byte[16]
            {
                0x2e, 0xc8, 0xe1, 0xad, 0x93, 0x2e, 0xc3, 0x7c, 0xb0, 0x28, 0xc3, 0x83, 0xe6, 0x79, 0xb0, 0x3b
            };
            byte[] result = HashingProvider.HashPassword("test string", salt);
            Assert.AreEqual(HashingProvider.KeyLength, result.Length);
        }

        [TestMethod]
        public void TestHashingProviderGenerateAccessToken()
        {
           string token = HashingProvider.GenerateAccessToken();
           Assert.AreEqual(HashingProvider.AccessTokenLength, token.Length);
           Assert.IsTrue(token.All((c) => Char.IsLetterOrDigit(c)));
        }
    }
}
