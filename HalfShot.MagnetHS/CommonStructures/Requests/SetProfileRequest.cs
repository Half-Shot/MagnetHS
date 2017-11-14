using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;

namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    [Serializable]
    public class SetProfileRequest : MQRequest
    {
        public UserID UserId { get; set; }
        public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
        public SetProfileRequest()
        {
            requestType = 0x11;
        }
    }
}
