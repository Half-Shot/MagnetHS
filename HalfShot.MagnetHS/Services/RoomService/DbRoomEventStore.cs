using System;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using System.Collections.Generic;
using System.Linq;
using HalfShot.MagnetHS.CommonStructures.Requests.Datastore;
using HalfShot.MagnetHS.CommonStructures.Responses.Datastore;

namespace HalfShot.MagnetHS.RoomService
{
    class DbRoomEventStore : IRoomEventStore
    {
        public DbRoomEventStore()
        {
            
        }

        public IEnumerable<PDUEvent> GetEvent(RoomID roomId, params EventID[] eventId)
        {
            using (var DbQueue = MQConnector.GetRequester(EMQService.Datastore))
            {
                DbQueue.Request(new GetEventsRequest()
                {
                    RoomId = roomId,
                    EventsToGet = new List<EventID>(eventId)
                });
                try
                {
                    var response = DbQueue.ListenForResponse();
                    if (response is StatusResponse)
                    {
                        throw new Exception($"Could not retrieve events from graph: {(response as StatusResponse).Error}");
                    }
                    var evtResponse = response as EventsResponse;
                    return evtResponse.Events;
                }
                catch (TimeoutException e)
                {
                    throw;
                }
            }
        }
        
        public PDUEvent GetStateEvent(RoomID roomId, string type, string stateKey = null, int stateDepth = 0)
        {
            using (var DbQueue = MQConnector.GetRequester(EMQService.Datastore))
            {
                DbQueue.Request(new GetFilteredEventsRequest()
                {
                    RoomId = roomId,
                    Type = type,
                    StateKey = stateKey,
                    Depth = stateDepth,
                });
                try
                {
                    var response = DbQueue.ListenForResponse();
                    if (response is StatusResponse)
                    {
                        throw new Exception(
                            $"Could not retrieve events from graph: {(response as StatusResponse).Error}");
                    }
                    var evtResponse = response as EventsResponse;
                    return evtResponse.Events.FirstOrDefault();
                }
                catch (TimeoutException e)
                {
                    throw;
                }
            }
        }

        public void PutEvent(params PDUEvent[] evs)
        {
            using (var DbQueue = MQConnector.GetRequester(EMQService.Datastore))
            {
                DbQueue.Request(new InsertEventsRequest()
                {
                    Events = new List<PDUEvent>(evs)
                });
                try
                {
                    var response = DbQueue.ListenForResponse() as StatusResponse;
                    if (!response.Succeeded)
                    {
                        throw new Exception($"Could not save graph events to DB: {(response).Error}");
                    }
                }
                catch (TimeoutException e)
                {
                    throw;
                }
            }
        }
    }
}