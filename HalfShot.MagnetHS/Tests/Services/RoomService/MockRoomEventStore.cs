using System;
using System.Linq;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.Tests.Services.RoomService
{
    internal class MockRoomEventStore : IRoomEventStore
    {
        private readonly List<PDUEvent> events;

        public MockRoomEventStore()
        {
            events = new List<PDUEvent>();
        }

        public IEnumerable<PDUEvent> GetEvent(RoomID roomId, params EventID[] eventId)
        {
            return events.Where((ev) => eventId.Any((_ev) => _ev == ev.EventId)).AsEnumerable();
        }

        public PDUEvent GetStateEvent(RoomID roomId, string type, string stateKey = null, int stateDepth = 0)
        {
            return events.Where((ev) => ev.RoomId == roomId &&
                                        ev.Type == type && 
            (stateKey == null) || (ev.StateKey == stateKey)
            ).Skip(stateDepth).FirstOrDefault();
        }

        public void PutEvent(params PDUEvent[] evs)
        {
            events.AddRange(evs);
        }
    }
}