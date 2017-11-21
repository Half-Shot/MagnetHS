using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Responses.Room
{
    [Serializable]
    public class CreateRoomResponse : MessageQueue.MQResponse
    {
        public RoomID RoomId { get; set; } 
        public List<Events.ClientEvent> BaseEvents { get; set; }
    }
}