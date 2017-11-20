using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Responses
{
    [Serializable]
    public class AccessTokenResponse : MessageQueue.MQResponse
    {
        public UserID UserId;
        public Dictionary<string, string> AccessTokenDevices;
        public HashSet<string> AccessTokens => new HashSet<string>(AccessTokenDevices.Keys);
        public HashSet<string> DeviceIds => new HashSet<string>(AccessTokenDevices.Values);
    }
}
