    using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests.Datastore
{
    [Serializable]
    public class GetEventsRequest : MessageQueue.MQRequest
    {
        public List<EventID> EventsToGet { get; set; }
        public RoomID RoomId {get;set;}
    }
}