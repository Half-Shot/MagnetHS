using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests.Room
{
    [Serializable]
    public class CreateRoomRequest : MessageQueue.MQRequest
    {
        UserID Sender;
    }
}
