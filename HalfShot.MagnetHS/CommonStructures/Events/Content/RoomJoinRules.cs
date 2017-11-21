using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Enums;

namespace HalfShot.MagnetHS.CommonStructures.Events.Content
{
    [Serializable]
    public class RoomJoinRules
    {
        public EJoinRule JoinRule { get; set; }
    }
}
