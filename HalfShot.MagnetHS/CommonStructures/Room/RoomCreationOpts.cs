using System;
using System.Linq;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using System.Collections.Generic;
using HalfShot.MagnetHS.CommonStructures.Enums;

namespace HalfShot.MagnetHS.CommonStructures.Room
{
    [Serializable]
    public class RoomCreationOpts
    {
        public string Name { get; set; } = null;
        public string Topic { get; set; } = null;
        public string RoomAliasName { get; set; } = null;
        public Dictionary<string, object> CreationContent { get; set; }
        public List<UserID> Invitees { get; set; } = new List<UserID>();
        public EPublished Visibility { get; set; }
        public bool IsDirect = true;
        //TODO: Initial_state, Invite_3pid present
    }
}