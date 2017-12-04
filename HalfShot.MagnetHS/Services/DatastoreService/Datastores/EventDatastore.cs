using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Requests.Datastore;
using HalfShot.MagnetHS.CommonStructures.Responses.Datastore;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.DatastoreService.Contexts;
using HalfShot.MagnetHS.DatastoreService.Records;
using HalfShot.MagnetHS.CommonStructures.Events;
using System.Threading.Tasks;

namespace HalfShot.MagnetHS.DatastoreService.Datastores
{
    class EventDatastore : IDatastore
    {
        private bool RoomExists(RoomID roomId)
        {
            using (var roomStore = new RoomStoreContext())
            {
                return roomStore.Events.Any((record) => record.RoomId == roomId.ToString());
            }
        }

        private MQResponse GetEvents(GetEventsRequest request)
        {
            if (!RoomExists(request.RoomId))
            {
                return new StatusResponse()
                {
                    ErrorCode = StatusResponse.NotFound,
                    Error = "No room found in store",
                    Succeeded = false,
                };
            }
            List<string> eventToGet = request.EventsToGet.ConvertAll((ev) => ev.ToString());
            List<EventRecord> eventRecords;
            List<EventEdgeRecord> eventEdgeRecords;
            List<EventHashRecord> hashRecords;
            using (var roomStore = new RoomStoreContext())
            {
                eventRecords = (from record in roomStore.Events
                          where eventToGet.Contains(record.EventId)
                          select record).ToList();
                eventEdgeRecords = (from record in roomStore.EventEdges
                                    where eventToGet.Contains(record.EventId)
                                    select record).ToList();
                eventToGet.AddRange(eventEdgeRecords.Select((row) => row.PrevEventId));
                hashRecords = (from record in roomStore.EventHashes
                           where eventToGet.Contains(record.EventId)
                           select record).ToList();
            }
            List<PDUEvent> pduEvents = new List<PDUEvent>();
            foreach (EventRecord evRecord in eventRecords)
            {
                var pduEvent = HidrateEvent(evRecord);
                pduEvent.Hashes = new EventHash();
                pduEvent.Hashes.SHA256 = hashRecords.First((record) => evRecord.EventId == record.EventId && record.HashType == "SHA256").Value;
                //Get parents
                foreach (EventEdgeRecord edge in eventEdgeRecords.Where((record) => evRecord.EventId == record.EventId))
                {
                    var hashRecord = hashRecords.First((record) => evRecord.EventId == record.EventId && record.HashType == "SHA256");
                    pduEvent.PreviousEvents.Add(new EventHash()
                    {
                        EventId = new EventID(edge.PrevEventId),
                        SHA256 = hashRecord.Value
                    });
                }
            }
            return new EventsResponse()
            {
                Events = pduEvents
            };
        }

        private StatusResponse InsertEvents(InsertEventsRequest request)
        {
            List<Task> tasks = new List<Task>();
            using (var roomStore = new RoomStoreContext())
            {
                foreach (PDUEvent pduEvent in request.Events)
                {
                    if (roomStore.Find<PDUEvent>(pduEvent.EventId.ToString()) != null)
                    {
                        throw new InvalidOperationException("Cannot insert the same eventid twice!");
                    }
                    EventRecord record = new EventRecord()
                    {
                        Content = pduEvent.JsonContent,
                        Depth = pduEvent.Depth,
                        EventId = pduEvent.EventId.ToString(),
                        Origin = pduEvent.Origin,
                        OriginServerTs = pduEvent.OriginServerTs,
                        RoomId = pduEvent.RoomId.ToString(),
                        Sender = pduEvent.Sender.ToString(),
                        StateKey = pduEvent.StateKey,
                        Type = pduEvent.Type,
                    };
                    foreach (EventHash previous in pduEvent.PreviousEvents)
                    {
                        tasks.Add(roomStore.AddAsync(new EventEdgeRecord()
                        {
                            EventId = pduEvent.EventId.ToString(),
                            PrevEventId = previous.EventId.ToString()
                        }));
                    }
                    tasks.Add(roomStore.AddAsync(new EventHashRecord()
                    {
                       EventId = pduEvent.EventId.ToString(),
                       HashType = "SHA256",
                       Value = pduEvent.Hashes.SHA256,
                    }));
                    tasks.Add(roomStore.AddAsync(record));
                }
                Task.WhenAll(tasks).Wait();
                roomStore.SaveChanges();
                return new StatusResponse()
                {
                    Succeeded = true,
                };
            }
        }

        private PDUEvent HidrateEvent(EventRecord eventRecord)
        {
            PDUEvent pduEvent = new PDUEvent();
            pduEvent.JsonContent = eventRecord.Content;
            pduEvent.EventId = new EventID(eventRecord.EventId);
            pduEvent.Depth = eventRecord.Depth;
            pduEvent.Origin = eventRecord.Origin;
            pduEvent.OriginServerTs = eventRecord.OriginServerTs;
            pduEvent.Sender = new UserID(eventRecord.Sender);
            pduEvent.StateKey = eventRecord.StateKey;
            pduEvent.Type = eventRecord.Type;
            return pduEvent;
        }

        public bool CanHandleRequest(MQRequest request)
        {
            return (request is GetEventsRequest) || (request is InsertEventsRequest);
        }

        public MQResponse RouteRequest(MQRequest request)
        {
            try
            {
                if (request is GetEventsRequest)
                {
                    return GetEvents(request as GetEventsRequest);
                }
                else if (request is InsertEventsRequest)
                {
                    return InsertEvents(request as InsertEventsRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Encountered an exeption while doing a DB operation. {ex.Message}");
                return new StatusResponse() { Succeeded = false, ErrorCode = "HS_DBERROR", Error = ex.Message };
            }
            return null;
        }
    }
}
