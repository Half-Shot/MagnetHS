using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.CommonStructures;
namespace HalfShot.MagnetHS.Tests.CommonStructures
{
    [TestClass]
    public class TestCommonIdentifier
    {
        [TestMethod]
        public void TestGoodUserId()
        {
            new UserID("@test:localhost");
            new UserID("@test:localhost.com");
            new UserID("@test:localhost.com:5544");
            new UserID("@goodboye:localhost.com:5544");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestBadUserIdAt()
        {
            new UserID("test:localhost");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestBadUserIdNoDomain()
        {
            new UserID("@test");
        }

        [TestMethod]
        public void TestUserIdString()
        {
            Assert.AreEqual(new UserID("@test:localhost").ToString(), "@test:localhost");
            Assert.AreEqual(new UserID("@test:localhost:5678").ToString(), "@test:localhost:5678");
        }

        [TestMethod]
        public void TestUserIdEquality()
        {
            Assert.IsTrue(new UserID("@test:localhost") == new UserID("@test:localhost"));
            Assert.IsFalse(new UserID("@test:localhost") == new UserID("@dog:localhost"));
            Assert.IsFalse(new UserID("@test:localhost") == new UserID("@test:mutt"));
        }

        [TestMethod]
        public void TestUserIdEqualityNull()
        {
            Assert.IsFalse(new UserID("@test:localhost") == null);
            Assert.IsTrue(new UserID("@test:localhost") != null);
        }
    }
}
