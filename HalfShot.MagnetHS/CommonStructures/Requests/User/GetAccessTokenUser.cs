using System;

namespace HalfShot.MagnetHS.CommonStructures.Requests.User
{
    [Serializable]
    public class GetAccessTokenUser : MessageQueue.MQRequest
    {
        public string AccessToken { get; set; }
    }
}