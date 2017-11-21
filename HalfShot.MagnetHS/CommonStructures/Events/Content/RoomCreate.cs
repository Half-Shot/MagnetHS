using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events.Content
{
    [Serializable]
    public class RoomCreate
    {
        public UserID Creator { get; set; }
        public Boolean Federate { get; set; } = true;
    }
}
