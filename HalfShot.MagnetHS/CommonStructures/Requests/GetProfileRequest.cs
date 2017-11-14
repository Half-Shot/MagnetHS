using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;

namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    [Serializable]
    public class GetProfileRequest : MQRequest
    {
        public UserID UserId { get; set; }
        public string[] Keys { get; set; } = new string[0];
        public GetProfileRequest ()
        {
            requestType = 0x10;
        }
    }
}
