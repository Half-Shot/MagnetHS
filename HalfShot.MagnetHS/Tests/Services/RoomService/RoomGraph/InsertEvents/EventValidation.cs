using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Events.Content;
using HalfShot.MagnetHS.RoomService;


namespace HalfShot.MagnetHS.Tests.Services.RoomService.InsertEvents
{
    [TestClass]
    /***
    This class tests the initial validation and adding events to the graph.
    ***/
    public class TestRoomGraph_InsertEvents_EventValidation
    {
        
        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_BadRoomId(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent(){
                Type = "m.room.message",
                Sender = TestRoomGraph.creatorUser,
                RoomId = new RoomID("!badid:localhost"),
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new { body = "Hello world!" }
            });
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_NullSender(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent(){
                Type = "m.room.message",
                Sender = null,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new { body = "Hello world!" }
            });
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_EmptyType(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent(){
                Type = " ",
                Sender = TestRoomGraph.creatorUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new { body = "Hello world!" }
            });
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_NullType(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent(){
                Type = null,
                Sender = TestRoomGraph.creatorUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new { body = "Hello world!" }
            });
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_NullEventIds(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent(){
                Type = "m.room.message",
                Sender = TestRoomGraph.creatorUser,
                RoomId = TestRoomGraph.roomId,
                EventId = null,
                Content = new { body = "Hello world!" }
            });
        }

        [TestMethod]
        public void TestInsertEventsPDU(){
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent(){
                Type = "m.room.message",
                Sender = TestRoomGraph.creatorUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new { body = "Hello world!" }
            });
            Assert.AreEqual(graph.Depth, 5);
        }
    }
}