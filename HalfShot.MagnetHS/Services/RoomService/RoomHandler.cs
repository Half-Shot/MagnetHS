using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Events.Content;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Room;
namespace HalfShot.MagnetHS.RoomService
{
    public class RoomHandler
    {
        Dictionary<RoomID, RoomGraph> roomGraphs;
        private IMessageQueue userQueue;
        private string domain;
        public RoomHandler()
        {
            
        }

        public RoomID CreateRoom(UserID creator, RoomCreationOpts opts)
        {
            RoomID roomId = RoomID.Generate(domain);
            RoomGraph graph = new RoomGraph(roomId);

            RoomMember creatorMember = new RoomMember() {Membership = EMembership.Join};
            // Get user profile.
            using (var userQueue = MQConnector.GetRequester(EMQService.User))
            {
                userQueue.Request(new GetProfileRequest()
                {
                    Keys = new string[] { "displayname", "avatar_url" },
                    UserId = creator
                });
                var response = userQueue.ListenForResponse();
                if (response is ProfileResponse)
                {
                    var creatorProfile = (response as ProfileResponse).Profile;
                    creatorMember.DisplayName = creatorProfile.Profile.GetValueOrDefault("displayname", null);
                    creatorMember.AvatarUrl = creatorProfile.Profile.GetValueOrDefault("avatar_url", null);
                }
            }
            // Insert creation event.
            graph.InsertEvents(new PDUEvent()
            {
                Type = "m.room.create",
                Depth = 0
            },
            // Insert join
            new PDUEvent()
            {
                Type = "m.room.member",
                Sender = creator,
                RoomId = roomId,
                Content = creatorMember,
                EventId = EventID.Generate(domain),
                StateKey = creator.ToString(),
            },
            // Insert join rules
            new PDUEvent()
            {
                Type = "m.room.join_rules",
                Sender = creator,
                RoomId =  roomId,
                Content = new RoomJoinRules()
                {
                    JoinRule = EJoinRule.Private
                }
            },
            // Insert power levels
            new PDUEvent()
            {
                Type = "m.room.power_levels",
                Sender = creator,
                RoomId =  roomId,
                Content = new RoomPowerLevels()
                {
                    Users = { { creator.ToString(), 100 }}
                }
            });
            if (opts.Name != null)
            {
                graph.InsertEvents(new PDUEvent()
                {
                    Type = "m.room.name",
                    Sender = creator,
                    RoomId =  roomId,
                    Content = new
                    {
                        name = opts.Name
                    }
                });
            }
            if (opts.Topic != null)
            {
                graph.InsertEvents(new PDUEvent()
                {
                    Type = "m.room.topic",
                    Sender = creator,
                    RoomId =  roomId,
                    Content = new
                    {
                        topic = opts.Topic
                    }
                });
            }
            roomGraphs.Add(roomId, graph);
            return roomId;
        }

        public void DeployRoomGraph(RoomID roomId)
        {
            if (!roomGraphs.ContainsKey(roomId))
            {
                RoomGraph graph = new RoomGraph(roomId, true);
                graph.FetchState();
                roomGraphs.Add(roomId, graph);
            }
        }
    }
}