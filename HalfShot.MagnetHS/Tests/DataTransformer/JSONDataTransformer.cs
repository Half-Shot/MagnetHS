using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.DataTransformer;
using System.IO;
namespace HalfShot.MagnetHS.Tests.DataTransformer
{
    [Serializable]
    class TestJSONDataTransformerObj
    {
        public string TestString { get; set; }
    }

    [TestClass]
    [TestCategory("DataTransformer")]
    public class TestJSONDataTransformer
    {
        [TestMethod]
        public void TestJSONFromStream()
        {
            using (StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(@"
{
    ""TestString"":""123""
}"))))
            {
                var obj = new JSONDataTransformer().FromStream<TestJSONDataTransformerObj>(reader);
                Assert.AreEqual("123", obj.TestString);
            }
        }

        [TestMethod]
        public void TestJSONConvertToString()
        {
            string result = new JSONDataTransformer().ConvertToString(new TestJSONDataTransformerObj() { TestString = "AWonderfulTest" });
            Assert.AreEqual("{\"TestString\":\"AWonderfulTest\"}", result);
        }

        [TestMethod]
        public void TestJSONConvertToBytes()
        {
            var obj = new JSONDataTransformer().ToBytes(new TestJSONDataTransformerObj() { TestString = "AWonderfulTest" });
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Length > 0);
        }

        [TestMethod]
        public void TestJSONConvertToStream()
        {
            using (var stream = new JSONDataTransformer().ToStream(new TestJSONDataTransformerObj() { TestString = "AWonderfulTest" }))
            {
                Assert.IsTrue(stream.Length > 0);
            }
        }
    }
}
