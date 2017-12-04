using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Events;

namespace HalfShot.MagnetHS.CommonStructures.Room
{
    public interface IRoomEventStore
    {
        IEnumerable<PDUEvent> GetEvent(RoomID roomId, params EventID[] eventId);
        PDUEvent GetStateEvent(RoomID roomId, string type, string stateKey = null, int stateDepth = 0);
        void PutEvent(params PDUEvent[] evs);
    }
}