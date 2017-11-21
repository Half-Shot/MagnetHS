using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Events.Content;
using HalfShot.MagnetHS.RoomService;


namespace HalfShot.MagnetHS.Tests.Services.RoomService
{
    [TestClass]
    public class TestRoomGraph
    {
        static RoomID goodId = new RoomID("!abc:localhost.com");
        static UserID creatorUser = new UserID("@admin:localhost.com");
        static UserID joinerUser = new UserID("@joiner:localhost.com");

        #region IsEventAuthorised
        [TestMethod]
        public void TestRoomGraphIsEventAuthorised_MRoomCreate()
        {
            RoomGraph graph = new RoomGraph(goodId, false);
            Assert.IsTrue(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 0,
                Type = "m.room.create"
            }));
            Assert.IsFalse(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 1,
                Type = "m.room.create"
            }));
        }

        [TestMethod]
        // If the only previous event is an m.room.create and the state_key is the creator, allow.
        public void TestRoomGraphIsEventAuthorised_MemberJoinFirst()
        {
            RoomGraph graph = new RoomGraph(goodId, false);
            AddCreationEvent(graph);
            Assert.IsTrue(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 1,
                Type = "m.room.member",
                StateKey = creatorUser.ToString(),
                Sender = creatorUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
            Assert.IsFalse(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 2,
                Type = "m.room.member",
                StateKey = creatorUser.ToString(),
                Sender = creatorUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
            Assert.IsFalse(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 1,
                Type = "m.room.member",
                StateKey = creatorUser.ToString(),
                Sender = joinerUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
            Assert.IsFalse(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 1,
                Type = "m.room.member",
                StateKey = joinerUser.ToString(),
                Sender = joinerUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
        }

        [TestMethod]
        // If the sender does not match state_key, reject.
        public void TestRoomGraphIsEventAuthorised_MemberJoinSenderMatch()
        {
            RoomGraph graph = new RoomGraph(goodId, false);
            AddCreationEvent(graph);
            Assert.IsFalse(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 1,
                Type = "m.room.member",
                StateKey = "@bad:localhost",
                Sender = creatorUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
            graph = new RoomGraph(goodId, false);
            AddJoinRules(graph, EJoinRule.Public);
            Assert.IsTrue(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 1,
                Type = "m.room.member",
                StateKey = "@bad:localhost",
                Sender = creatorUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
            /*AddMemberEvent(graph, new UserID("@newuser:localhost"), EMembership.Invite);
            Assert.IsTrue(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 2,
                Type = "m.room.member",
                StateKey = "@newuser:localhost",
                Sender = creatorUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));/*
        }

        /*[TestMethod]
        public void TestRoomGraphIsEventAuthorised_MemberJoinSomething()
        {
            graph = new RoomGraph(goodId, false);
            AddCreationEvent(graph);
            AddMemberEvent(graph, new UserID("@newuser:localhost"), EMembership.Join);
            Assert.IsTrue(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 2,
                Type = "m.room.member",
                StateKey = joinerUser.ToString(),
                Sender = creatorUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
            graph = new RoomGraph(goodId, false);
            AddCreationEvent(graph);
            AddJoinRules(graph);
            Assert.IsTrue(graph.IsEventAuthorised(new PDUEvent()
            {
                Depth = 2,
                Type = "m.room.member",
                StateKey = joinerUser.ToString(),
                Sender = joinerUser,
                Content = new RoomMember()
                {
                    Membership = EMembership.Join
                }
            }));
        }*/
            #endregion


            private void AddCreationEvent(RoomGraph graph)
        {
            graph.InsertEvent(new PDUEvent()
            {
                Depth = 0,
                Type = "m.room.create",
                Sender = creatorUser,
                Content = new RoomCreate()
                {
                    Creator = creatorUser,
                    Federate = false
                }
            });
        }

        private void AddJoinRules(RoomGraph graph, EJoinRule rule = EJoinRule.Public)
        {
            graph.InsertEvent(new PDUEvent()
            {
                Depth = graph.Depth + 1,
                Type = "m.room.join_rules",
                Sender = creatorUser,
                Content = new RoomJoinRules()
                {
                    JoinRule = rule
                }
            });
        }


        private void AddMemberEvent(RoomGraph graph, UserID userId, EMembership membership = EMembership.Join, UserID event_sender = null)
        {
            if(event_sender == null)
            {
                event_sender = creatorUser;
            }
            graph.InsertEvent(new PDUEvent()
            {
                Depth = graph.Depth + 1,
                Type = "m.room.create",
                Sender = event_sender,
                StateKey = userId.ToString(),
                Content = new RoomMember()
                {
                    Membership = membership
                }
            });
        }
    }
}
