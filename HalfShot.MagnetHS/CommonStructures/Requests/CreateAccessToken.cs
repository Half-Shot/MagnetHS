using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    [Serializable]
    public class CreateAccessTokenRequest : MessageQueue.MQRequest
    {
        public UserID UserId { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public string DeviceId { get; set; }
    }
}
