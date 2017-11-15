using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.ClientServerAPIService;
namespace HalfShot.MagnetHS.Tests.Services.ClientServerAPIService
{
    [TestClass]
    public class TestRestContext
    {
        [TestMethod]
        public void TestProcessPathParametersEmpty()
        {
            RestContext context = new RestContext();
            context.ProcessPathParameters(new Regex("/abc/def/ghi"), "/abc/def/ghi");
            Assert.AreEqual(0, context.PathParameters.Count);
        }

        [TestMethod]
        public void TestProcessPathParametersOne()
        {
            RestContext context = new RestContext();
            context.ProcessPathParameters(new Regex("/abc/(?<key>.+)/ghi"), "/abc/def/ghi");
            Assert.AreEqual(1, context.PathParameters.Count);
            Assert.AreEqual("def", context.PathParameters["key"]);
        }

        [TestMethod]
        public void TestProcessPathParametersMultiple()
        {
            RestContext context = new RestContext();
            context.ProcessPathParameters(new Regex("/(?<key>.+)/def/(?<key2>.+)"), "/abc/def/ghi");
            Assert.AreEqual(2, context.PathParameters.Count);
            Assert.AreEqual("abc", context.PathParameters["key"]);
            Assert.AreEqual("ghi", context.PathParameters["key2"]);
        }
    }
}
