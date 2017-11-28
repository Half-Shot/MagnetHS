using System;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.RoomService
{
    class DbRoomEventStore : IRoomEventStore
    {
        public IMessageQueue DbQueue;
        public DbRoomEventStore()
        {
            DbQueue = MQConnector.GetRequester(EMQService.Datastore);
        }

        public IEnumerable<PDUEvent> GetEvent(params EventID[] eventId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PDUEvent> GetEvent(params string[] eventId)
        {
            throw new NotImplementedException();
        }

        public PDUEvent GetStateEvent(string type, string stateKey = null, int stateDepth = 0)
        {
            throw new NotImplementedException();
        }

        public void PutEvent(params PDUEvent[] evs)
        {
            throw new NotImplementedException();
        }
    }
}