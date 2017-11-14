using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.CommonStructures.Caches;
using HalfShot.MagnetHS.CommonStructures;

namespace HalfShot.MagnetHS.Tests.CommonStructures.Caches
{
    [TestClass]
    public class TestProfileCache
    {
        static UserID id = new UserID("@test:localhost");
        [TestMethod]
        public void TestCreateProfileCache()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.MaxValue);
        }

        [TestMethod]
        public void TestProfileCacheCreateProfile()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.MaxValue);
            cache.UpsertProfileValue(id, "testkey1", "testval1");
            cache.UpsertProfileValue(id, "testkey2", "testval2");
            cache.UpsertProfileValue(id, "testkey3", "testval3");
            UserProfile profile = cache.Get(id);
            Assert.AreEqual(profile.UserId, id);
            Assert.AreEqual("testval1", profile.Profile["testkey1"]);
            Assert.AreEqual("testval2", profile.Profile["testkey2"]);
            Assert.AreEqual("testval3", profile.Profile["testkey3"]);
        }

        [TestMethod]
        public void TestProfileCacheUpdate()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.MaxValue);
            cache.UpsertProfileValue(id, "testkey", "testval1");
            cache.UpsertProfileValue(id, "testkey", "testval2");
            UserProfile profile = cache.Get(id);
            Assert.AreEqual("testval2", profile.Profile["testkey"]);
        }

        [TestMethod]
        public void TestProfileCacheValid()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.MaxValue);
            cache.UpsertProfileValue(id, "testkey1", "testval1");
            cache.UpsertProfileValue(id, "testkey2", "testval2");
            Assert.IsFalse(cache.HasCacheExpired(id, new List<string>() { "testkey1", "testkey2" }));
            Assert.IsFalse(cache.HasCacheExpired(id, new List<string>() { }));
            Assert.IsFalse(cache.HasCacheExpired(id));
        }

        [TestMethod]
        public void TestProfileCacheInvalid()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.FromTicks(0));
            cache.UpsertProfileValue(id, "testkey1", "testval1");
            cache.UpsertProfileValue(id, "testkey2", "testval2");
            Assert.IsTrue(cache.HasCacheExpired(id, new List<string>() { "testkey1", "testkey2" }));
            Assert.IsTrue(cache.HasCacheExpired(id, new List<string>() { }));
            Assert.IsTrue(cache.HasCacheExpired(id));
        }

        [TestMethod]
        public void TestProfileCachePartialInvalid()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.FromMinutes(30));
            cache.UpsertProfileValue(id, "testkey1", "testval1");
            cache.InvalidateProfileValue(id, "testkey1");
            cache.UpsertProfileValue(id, "testkey2", "testval2");
            Assert.IsFalse(cache.HasCacheExpired(id, new List<string>() { "testkey2" }));
            Assert.IsTrue(cache.HasCacheExpired(id, new List<string>() { "testkey1"}));
            Assert.IsTrue(cache.HasCacheExpired(id, new List<string>() { "testkey1", "testkey2" }));
            Assert.IsTrue(cache.HasCacheExpired(id, new List<string>() { }));
            Assert.IsTrue(cache.HasCacheExpired(id));
        }

        [TestMethod]
        public void TestProfileCacheMissing()
        {
            ProfileCache cache = new ProfileCache(TimeSpan.MaxValue);
            Assert.IsTrue(cache.HasCacheExpired(id, new List<string>() { }));
            Assert.IsTrue(cache.HasCacheExpired(id));
        }
    }
}
