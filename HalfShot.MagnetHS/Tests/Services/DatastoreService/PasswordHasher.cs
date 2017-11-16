using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.DatastoreService;

namespace HalfShot.MagnetHS.Tests.Services.DatastoreService
{
    [TestClass]
    public class TestPasswordHasher
    {
        [TestMethod]
        public void TestPasswordHasherFunction()
        {
            byte[] salt = new byte[16]
            {
                0x2e, 0xc8, 0xe1, 0xad, 0x93, 0x2e, 0xc3, 0x7c, 0xb0, 0x28, 0xc3, 0x83, 0xe6, 0x79, 0xb0, 0x3b
            };
            byte[] result = PasswordHasher.HashPassword("test string", salt);
            Assert.AreEqual(256, result.Length);
        }
    }
}
