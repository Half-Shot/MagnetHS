using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.RoomService;

namespace HalfShot.MagnetHS.Tests.Services.RoomService
{
    [TestClass]
    public class TestRoomGraph
    {
        public static RoomID roomId = new RoomID("!abc:localhost");
        public static UserID creatorUser = new UserID("@admin:localhost");
        public static UserID joinerUser = new UserID("@joiner:localhost");

        [TestMethod]
        public void TestEnsure_BuildStandardRoomGraph(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                roomId,
                creatorUser
            );
            Assert.AreEqual(4, graph.Depth);
        }
    }
}
