using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Events;

namespace HalfShot.MagnetHS.CommonStructures.Room
{
    public interface IRoomEventStore
    {
        IEnumerable<PDUEvent> GetEvent(params EventID[] eventId);
        IEnumerable<PDUEvent> GetEvent(params string[] eventId);
        PDUEvent GetStateEvent(string type, string stateKey = null, int stateDepth = 0);
        void PutEvent(params PDUEvent[] evs);
    }
}