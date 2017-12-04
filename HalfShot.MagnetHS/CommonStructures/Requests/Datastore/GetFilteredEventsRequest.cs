    using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests.Datastore
{
    [Serializable]
    public class GetFilteredEventsRequest : MessageQueue.MQRequest
    {
        public string Sender { get; set; } = null;
        public string StateKey { get; set; } = null;
        public string Content { get; set; } = null;
        public string Type { get; set; } = null;
        public string Origin { get; set; } = null;
        public int Depth { get; set; } = 0;
        public RoomID RoomId {get; set;}
    }
}