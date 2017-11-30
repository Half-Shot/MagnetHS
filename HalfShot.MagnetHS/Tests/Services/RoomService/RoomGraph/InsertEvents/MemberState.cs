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
    This class tests the various states for members in a room.
    ***/
    public class TestRoomGraph_InsertEvents_MemberState
    {
        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_SendUnjoinedUser()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.message",
                Sender = TestRoomGraph.joinerUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new {body = "Hello world!"}
            });
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void TestInsertEventsPDU_UserJoin_Private_Join()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Private
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }

        [TestMethod]
        public void TestInsertEventsPDU_UserJoin_Private_InviteJoin()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Invite
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.creatorUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Invite
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }
    }
}