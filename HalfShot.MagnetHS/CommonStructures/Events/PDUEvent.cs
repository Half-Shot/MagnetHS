using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    [Serializable]
    public class PDUEvent : BaseEvent
    {
        public RoomID RoomId { get; set; }
        public UserID Sender { get; set; }
        public EventID EventId { get; set; }
        public DateTime OriginServerTs { get; set; }
        public List<EventHash> PreviousEvents { get; } = new List<EventHash>();
        public int Depth { get; set; }
        public List<EventHash> AuthEvents { get; } = new List<EventHash>();
        public EventHash Hashes { get; set; }
        public List<EventHash> Signatures { get; } = new List<EventHash>();
        public string StateKey { get; set; }
        public EventID Redacts { get; set; }
        public object Unsigned { get; set; }

        public static explicit operator PDUEvent(ClientEvent v)
        {
            PDUEvent ev = new PDUEvent();
            ev.Content = v.Content;
            ev.EventId = v.EventId;
            ev.Destination = v.Destination;
            ev.Type = v.Type;
            ev.Sender = v.Sender;
            ev.StateKey = v.StateKey;
            ev.RoomId = v.RoomId;
            return ev;
        }
    }
}