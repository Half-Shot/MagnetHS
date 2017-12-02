using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events.Content
{
    [Serializable]
    public class RoomCreate : IEventContent
    {
        public UserID Creator { get; set; }
        public Boolean Federate { get; set; } = true;
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
