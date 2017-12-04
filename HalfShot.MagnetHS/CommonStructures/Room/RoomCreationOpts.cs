using System;
using System.Linq;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.RoomService
{
    public class RoomCreationOpts
    {
        public string Name { get; set; } = null;
        public string Topic { get; set; } = null;
    }
}