using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Events.Content;
using HalfShot.MagnetHS.RoomService;
using HalfShot.MagnetHS.RoomService.Exceptions;


namespace HalfShot.MagnetHS.Tests.Services.RoomService.InsertEvents
{
    [TestClass]
    /***
    This class tests the various states for members in a room.
    ***/
    public class TestRoomGraph_InsertEvents_MemberState
    {
        [ExpectUnauthorizedEvent("Sender's membership is not joined")]
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
        
        [TestMethod]
        public void TestInsertEventsPDU_Public_JoinSend()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            },new PDUEvent()
            {
                Type = "m.room.message",
                Sender = TestRoomGraph.joinerUser,
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new {body = "Hello world!"}
            });
        }

        [ExpectUnauthorizedEvent("Sender is not allowed to join this room.")]
        [TestMethod]
        public void TestInsertEventsPDU_Private_Join()
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
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }

        [TestMethod]
        public void TestInsertEventsPDU_Invite_Join()
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
                StateKey = TestRoomGraph.joinerUser.ToString(),
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
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }
        
        [ExpectUnauthorizedEvent("Sender is not allowed to join this room.")]
        [TestMethod]
        public void TestInsertEventsPDU_UserJoin_BadInvitee()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Invite
            );
            var joiner = new UserID("@baduser:localhost");
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.creatorUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Invite
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = joiner,
                StateKey = joiner.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }
        
        [TestMethod]
        public void TestInsertEventsPDU_Invite_Leave()
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
                StateKey = TestRoomGraph.joinerUser.ToString(),
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
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Leave
                }
            });
        }
        
        [TestMethod]
        public void TestInsertEventsPDU_Kick()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.creatorUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Leave
                }
            });
        }
        
        
        [TestMethod]
        public void TestInsertEventsPDU_UserJoin_Kick_Join()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.creatorUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Leave
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }
        
        [TestMethod]
        public void TestInsertEventsPDU_Ban()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.creatorUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Ban
                }
            });
        }
        
        [TestMethod]
        [ExpectUnauthorizedEvent("Sender is not allowed to join this room.")]
        public void TestInsertEventsPDU_Ban_Join()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }, new PDUEvent()
            {
                Type = "m.room.member",
                Sender = TestRoomGraph.creatorUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Ban
                }
            }, new PDUEvent(){
                Type = "m.room.member",
                Sender = TestRoomGraph.joinerUser,
                StateKey = TestRoomGraph.joinerUser.ToString(),
                RoomId = TestRoomGraph.roomId,
                EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            });
        }

        [TestMethod]
        [ExpectUnauthorizedEvent("Sender cannot kick this user, because they do not have the required power.")]
        public void TestInsertEventsPDU_KickLowerPower()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            RoomPowerLevels pl = graph.PowerLevels;
            pl.Kick = 50;
            pl.Users[TestRoomGraph.joinerUser.ToString()] = 49;
            var userB = new UserID("@user2:localhost");
            graph.InsertEvents(new PDUEvent()
                {
                    Type = "m.room.member",
                    Sender = TestRoomGraph.joinerUser,
                    StateKey = TestRoomGraph.joinerUser.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Join
                    }
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    Sender = userB,
                    StateKey = userB.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Join
                    }
                },new PDUEvent()
                {
                    Type = "m.room.power_levels",
                    Sender = TestRoomGraph.creatorUser,
                    StateKey = TestRoomGraph.creatorUser.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = pl
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    Sender = TestRoomGraph.joinerUser,
                    StateKey = userB.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Leave
                    }
                }
            );
        }
        
        [TestMethod]
        [ExpectUnauthorizedEvent("Sender cannot kick this user, because they do not have the required power.")]
        public void TestInsertEventsPDU_KickLowerThanUser()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            RoomPowerLevels pl = graph.PowerLevels;
            var userB = new UserID("@user2:localhost");
            pl.Kick = 25;
            pl.Users[TestRoomGraph.joinerUser.ToString()] = 25;
            pl.Users[userB.ToString()] = 25;
            graph.InsertEvents(new PDUEvent()
                {
                    Type = "m.room.member",
                    Sender = TestRoomGraph.joinerUser,
                    StateKey = TestRoomGraph.joinerUser.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Join
                    }
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    Sender = userB,
                    StateKey = userB.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Join
                    }
                },new PDUEvent()
                {
                    Type = "m.room.power_levels",
                    Sender = TestRoomGraph.creatorUser,
                    StateKey = TestRoomGraph.creatorUser.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = pl
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    Sender = TestRoomGraph.joinerUser,
                    StateKey = userB.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Leave
                    }
                }
            );
        }
        
        [TestMethod]
        public void TestInsertEventsPDU_KickHigherPower()
        {
            RoomGraph graph = RoomGraphTestUtil.BuildStandardRoomGraph(
                TestRoomGraph.roomId,
                TestRoomGraph.creatorUser,
                EJoinRule.Public
            );
            RoomPowerLevels pl = graph.PowerLevels;
            var userB = new UserID("@user2:localhost");
            pl.Kick = 50;
            pl.Users[TestRoomGraph.joinerUser.ToString()] = 50;
            pl.Users[userB.ToString()] = 26;
            graph.InsertEvents(new PDUEvent()
                {
                    Type = "m.room.member",
                    Sender = TestRoomGraph.joinerUser,
                    StateKey = TestRoomGraph.joinerUser.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Join
                    }
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    Sender = userB,
                    StateKey = userB.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Join
                    }
                },new PDUEvent()
                {
                    Type = "m.room.power_levels",
                    Sender = TestRoomGraph.creatorUser,
                    StateKey = TestRoomGraph.creatorUser.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = pl
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    Sender = TestRoomGraph.joinerUser,
                    StateKey = userB.ToString(),
                    RoomId = TestRoomGraph.roomId,
                    EventId = EventID.Generate(RoomGraphTestUtil.DOMAIN),
                    Content = new RoomMember()
                    {
                        Membership = EMembership.Leave
                    }
                }
            );
        }
    }
}