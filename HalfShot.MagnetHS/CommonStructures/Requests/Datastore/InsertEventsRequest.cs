using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Events;
namespace HalfShot.MagnetHS.CommonStructures.Requests.Datastore
{
    [Serializable]
    public class InsertEventsRequest : MessageQueue.MQRequest
    {
        public List<PDUEvent> Events { get; set; } = new List<PDUEvent>();
    }
}
