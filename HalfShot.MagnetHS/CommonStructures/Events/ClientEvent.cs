using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    [Serializable]
    public class ClientEvent : BaseEvent
    {
        public EventID EventId {get;set;}
        public RoomID RoomId {get;set;}
        public UserID Sender {get;set;}
        public DateTime OriginServerTs {get;set;}
    }
}