using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Responses.Room
{
    [Serializable]
    public class RoomResponse : MessageQueue.MQResponse
    {
        public RoomID RoomId { get; set; }
    }
}