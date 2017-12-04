using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Responses.User
{
    [Serializable]
    public class UserResponse : MessageQueue.MQResponse
    {
        public UserID UserId { get; set; } = null;
    }
}