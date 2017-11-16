using HalfShot.MagnetHS.CommonStructures.Enums;
using System;
using System.Collections.Generic;
using System.Text;
namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    [Serializable]
    public class LoginRequest : MessageQueue.MQRequest
    {
        public ELoginType Type { get; set;}
        public UserID UserId { get; set; }
        public string Token { get; set; }
    }
}
