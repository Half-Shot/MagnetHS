using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;
namespace HalfShot.MagnetHS.CommonStructures.Responses
{
    [Serializable]
    public class ProfileResponse : MQResponse
    {
        public UserProfile Profile { get; }

        public ProfileResponse(UserProfile profile)
        {
            Profile = profile;
        }
    }
}
