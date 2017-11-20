using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    public class PDUEvent : BaseEvent
    {
        public RoomID RoomID;
        public UserID Sender;
        public EventID EventId;
        public DateTime OriginServerTs;
        public List<EventHash> PreviousEvents;
        public int Depth;
        public List<EventHash> AuthEvents;
        public EventHash Hashes;
        public List<EventHash> Signatures;
        public string StateKey;
        public EventID Redacts;
        public object Unsigned;
    }
}