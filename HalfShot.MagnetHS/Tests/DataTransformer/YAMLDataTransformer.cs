using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.DataTransformer;
using System.IO;
namespace HalfShot.MagnetHS.Tests.DataTransformer
{
    [Serializable]
    class TestYAMLDataTransformerObj
    {
        public string TestString { get; set; }
    }

    [TestClass]
    [TestCategory("DataTransformer")]
    public class TestYAMLDataTransformer
    {
        [TestMethod]
        public void TestYAMLFromStream()
        {
            using (StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(@"
TestString: 123
"))))
            {
                var obj = new YAMLDataTransformer().FromStream<TestYAMLDataTransformerObj>(reader);
                Assert.AreEqual("123", obj.TestString);
            }
        }

        [TestMethod]
        public void TestYAMLConvertToString()
        {
            var obj = new YAMLDataTransformer().ConvertToString(new TestJSONDataTransformerObj() { TestString = "AWonderfulTest" });
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Contains("AWonderfulTest"));
        }

        [TestMethod]
        public void TestYAMLConvertToBytes()
        {
            var obj = new YAMLDataTransformer().ToBytes(new TestJSONDataTransformerObj() { TestString = "AWonderfulTest" });
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Length > 0);
        }

        [TestMethod]
        public void TestYAMLConvertToStream()
        {
            using (var stream = new YAMLDataTransformer().ToStream(new TestYAMLDataTransformerObj() { TestString = "AWonderfulTest" }))
            {
                Assert.IsTrue(stream.Length > 0);
            }
        }
    }
}
