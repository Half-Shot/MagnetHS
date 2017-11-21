using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
namespace HalfShot.MagnetHS.RoomService
{
    class RoomHandler
    {
        Dictionary<RoomID, RoomGraph> roomGraphs;

        public RoomHandler()
        {

        }

        private void CreateRoom()
        {
            RoomID roomId = RoomID.GenerateRoomId("localhost");
            RoomGraph graph = new RoomGraph(roomId);
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
            },
            // Insert join rules
            new PDUEvent()
            {
                Type = "m.room.join_rules",
            },
            // Insert power levels
            new PDUEvent()
            {
                Type = "m.room.power_levels"
            });
            roomGraphs.Add(roomId, graph);
        }

        private void DeployRoomGraph(RoomID roomId)
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