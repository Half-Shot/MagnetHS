using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests.Federation
{
    [Serializable]
    public class GetEventsRequest : MessageQueue.MQRequest
    {
        public List<EventID> EventsToGet { get; set; }
    }
}
