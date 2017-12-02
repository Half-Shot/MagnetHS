using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Enums;

namespace HalfShot.MagnetHS.CommonStructures.Events.Content
{
    [Serializable]
    public class RoomMember : IEventContent
    {
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public EMembership Membership { get; set; }
    
        public bool IsDirect { get; set; }
        //public ThirdPatyInvite
        public void FromJsonContent(string json)
        {
            throw new NotImplementedException();
        }

        public object ToCanonicalObject()
        {
            throw new NotImplementedException();
        }
    }
}
