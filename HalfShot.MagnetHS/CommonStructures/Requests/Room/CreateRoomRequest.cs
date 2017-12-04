using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Room;

namespace HalfShot.MagnetHS.CommonStructures.Requests.Room
{
    [Serializable]
    public class CreateRoomRequest : MessageQueue.MQRequest
    {
        public UserID Sender { get; set; }
        public RoomCreationOpts Opts { get; set; }
    }
}
