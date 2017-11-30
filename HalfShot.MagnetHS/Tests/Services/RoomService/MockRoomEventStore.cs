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
    class MockRoomEventStore : IRoomEventStore
    {
        public List<PDUEvent> events;

        public MockRoomEventStore()
        {
            events = new List<PDUEvent>();
        }

        public IEnumerable<PDUEvent> GetEvent(params EventID[] eventId)
        {
            return GetEvent(eventId.ToString());
        }

        public IEnumerable<PDUEvent> GetEvent(params string[] eventId)
        {
            return events.Where((ev) => eventId.Contains(ev.EventId.ToString())).AsEnumerable();
        }

        public PDUEvent GetStateEvent(string type, string stateKey = null, int stateDepth = 0)
        {
            return events.Where((ev) => ev.Type == type && 
            (stateKey == null) || (ev.StateKey == stateKey)
            ).Skip(stateDepth).FirstOrDefault();
        }

        public void PutEvent(params PDUEvent[] evs)
        {
            events.AddRange(evs);
        }
    }
}