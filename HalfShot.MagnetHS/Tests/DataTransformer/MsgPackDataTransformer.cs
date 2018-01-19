using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.DataTransformer;
using System.IO;
using HalfShot.MagnetHS.CommonStructures;

namespace HalfShot.MagnetHS.Tests.DataTransformer
{
    [Serializable]
    public class TestMsgPackDataTransformerObj
    {
        public string TestString { get; set; }
        public UserID UserId { get; set; }
    }
    
    [TestClass]
    [TestCategory("DataTransformer")]
    public class TestMsgPackDataTransformer
    {
        [TestMethod]
        public void TestMsgPackFromStream()
        {

        }

        [TestMethod]
        public void TestMsgPackConvertToString()
        {
            var obj = new MsgPackDataTransformer().ConvertToString(
                new TestMsgPackDataTransformerObj()
                {
                    TestString = "AWonderfulTest",
                    UserId = new UserID("@123:abc.com")
                }
            );
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Contains("kq5BV29uZGVyZnVsVGVzdJKnYWJjLmNvbaMxMjM="));
        }

        [TestMethod]
        public void TestMsgPackConvertToBytes()
        {
            var obj = new MsgPackDataTransformer().ToBytes(
                new TestMsgPackDataTransformerObj()
                {
                    TestString = "AWonderfulTest",
                    UserId = new UserID("@123:abc.com")
                }
            );
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.Length > 0);
        }

        [TestMethod]
        public void TestMsgPackConvertToStream()
        {
            using (var stream = new MsgPackDataTransformer().ToStream(
                new TestMsgPackDataTransformerObj()
                {
                    TestString = "AWonderfulTest",
                    UserId = new UserID("@123:abc.com")
                }
            )){
                Assert.IsTrue(stream.Length > 0);
            }
        }
    }
}