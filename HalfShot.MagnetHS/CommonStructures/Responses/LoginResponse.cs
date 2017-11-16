using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Responses
{
    [Serializable]
    public class LoginResponse : MessageQueue.MQResponse
    {
        public UserID UserId;
        public string AccessToken;
    }
}
