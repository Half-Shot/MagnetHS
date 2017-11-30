using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Enums;

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

        public string CalculateHash(EEventHashType eventHashType)
        {
            HashAlgorithm algo;
            switch (eventHashType)
            {
                    case EEventHashType.Sha256:
                        algo = SHA256.Create();
                        break;
                    default:
                        throw new NotSupportedException("This hash type is not supported");
            }
            //TODO: Missing membership
            //TODO: Missing state
            object hashableObj =
                new
                {
                    auth_events = AuthEvents,
                    depth = Depth,
                    event_id = EventId.ToString(),
                    hashes = Hashes,
                    origin = Origin,
                    origin_server_ts = OriginServerTs,
                    prev_events = PreviousEvents,
                    room_id = RoomId.ToString(),
                    sender = Sender.ToString(),
                    signatures = Signatures,
                    state_key = StateKey,
                    type = Type,
                };
            
            using (algo)
            {
                return Convert.ToBase64String(algo.ComputeHash(jsonTransformer.ToBytes(hashableObj)));   
            }
        }
    }
}