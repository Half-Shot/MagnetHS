using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;
namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    public class GetRoomEvents : MessageQueue.MQRequest
    {
        public UserID OnBehalfOf { get; set; }
        public RoomID RoomId { get; set; }
        public string StateKey = null;
        public int Limit = 10;
    }
}
