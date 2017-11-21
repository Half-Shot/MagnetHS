using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Events;

namespace HalfShot.MagnetHS.CommonStructures.Responses.Datastore
{
    [Serializable]
    public class EventsResponse : MessageQueue.MQResponse
    {
        public List<PDUEvent> Events { get; set; }
    }
}
