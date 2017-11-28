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
    static class RoomGraphTestUtil {
        public const string DOMAIN = "localhost";
        public static RoomGraph BuildStandardRoomGraph(RoomID roomId, UserID creator, EJoinRule joinRule = EJoinRule.Public) {
            MockRoomEventStore store = new MockRoomEventStore();
            RoomGraph graph = new RoomGraph(roomId);
            var powerLevels = new RoomPowerLevels();
            powerLevels.Users.Add(creator, 100);
            graph.SetEventSource(store);
            graph.InsertEvents(
                new PDUEvent(){
                    Type = "m.room.create",
                    EventId = EventID.Generate(DOMAIN),
                    RoomId = roomId,
                    Sender = creator
                },
                new PDUEvent(){
                    Type = "m.room.member",
                    EventId = EventID.Generate(DOMAIN),
                    RoomId = roomId,
                    Sender = creator,
                    StateKey = creator.ToString(),
                    Content = new RoomMember() {
                        Membership = EMembership.Join,
                    }
                },
                new PDUEvent(){
                    Type = "m.room.power_levels",
                    EventId = EventID.Generate(DOMAIN),
                    RoomId = roomId,
                    Sender = creator,
                    Content = powerLevels
                },
                new PDUEvent(){
                    Type = "m.room.join_rules",
                    EventId = EventID.Generate(DOMAIN),
                    RoomId = roomId,
                    Sender = creator,
                    Content = new RoomJoinRules() {
                        JoinRule = joinRule
                    }
                }
            );
            return graph;
        }
    }
}