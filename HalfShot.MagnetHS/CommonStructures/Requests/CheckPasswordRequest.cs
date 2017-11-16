using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    [Serializable]
    public class CheckPasswordRequest : MessageQueue.MQRequest
    {
        public UserID UserId { get; set; }
        public string Password { get; set; }
    }
}
