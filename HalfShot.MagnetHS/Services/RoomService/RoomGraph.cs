using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Requests.Datastore;
using HalfShot.MagnetHS.CommonStructures.Responses.Datastore;
using System.Linq;

namespace HalfShot.MagnetHS.RoomService
{
    public class RoomGraph
    {
        static IMessageQueue FederationRequest = Program.FederationRequest;
        static IMessageQueue DbQueue = Program.DbQueue;
        public RoomID RoomId { get; private set;}
        public EJoinRule JoinRule { get; private set; }
        public bool Federated {get; private set;}
        public int Depth { get; private set; }

        private Dictionary<UserID, EMembership> memberStates;

        private Dictionary<EventID, PDUEvent> eventCache;
        public RoomGraph(RoomID roomId, bool federated = true) {
            RoomId = roomId;
            eventCache = new Dictionary<EventID, PDUEvent>();
            Federated = federated;
        }
        

        public void InsertEvents(params PDUEvent[] pduEvents)
        {
            // Validate the event.
            // Get the tip of the graph.
            List<PDUEvent> events = pduEvents.ToList();
            foreach (var pduEvent in pduEvents)
            {
                if (pduEvent.RoomId != RoomId)
                {
                    throw new Exception("RoomId does not match.");
                }
                if (String.IsNullOrWhiteSpace(pduEvent.Type))
                {
                    throw new Exception("A type must be given.");
                }
            }
            DbQueue.Request(new InsertEventsRequest()
            {
                Events = events
            });
            StatusResponse response = DbQueue.ListenForResponse() as StatusResponse;
        }

        public void InsertEvent(ClientEvent clientEvent) {
            // Validate the event.
            // Transform the client event into a room event.
            PDUEvent pduEvent = (PDUEvent)clientEvent;
            //Add edges, hash in the data.
            //Fill in the rest.
            InsertEvents(pduEvent);
        }

        public void FetchState()
        {

        }

        private PDUEvent getEvent(EventID eventId){
            if(eventCache.ContainsKey(eventId)){
                return eventCache[eventId];
            }
            //TODO: Fetch from DB.
            DbQueue.Request(new GetEventsRequest()
            {
                RoomId = RoomId,
                EventsToGet = new List<EventID>() { eventId }
            });
            MQResponse response = DbQueue.ListenForResponse();
            if(response is StatusResponse)
            {
                throw new Exception((response as StatusResponse).Error);
            } else if (response is EventsResponse) {
                var eventResponse = response as EventsResponse;
                if(eventResponse.Events.Count == 1)
                {
                    eventCache.Add(eventResponse.Events[0].EventId, eventResponse.Events[0]);
                    return eventResponse.Events[0];
                } else {
                    throw new Exception("Unexpected count of events returned");
                }
            } else {
                throw new Exception("Unexpected response message from DBStore");
            }

            if (Federated) {
                //TODO: Fetch from Federation handler.
                //FederationRequest.Request();
                //FederationRequest.ListenForResponse();
            } else {
                return null;
            }
        }

        public bool IsEventAuthorised(PDUEvent ev)
        {
            if(ev.Type == "m.room.create" && ev.Depth != 0)
            {
                return false;
            }
            return false;
        }
    }
}