using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
namespace HalfShot.MagnetHS.RoomService
{
    class RoomGraph
    {
        static IMessageQueue FederationRequest = Program.FederationRequest;
        static IMessageQueue DbQueue = Program.DbQueue;
        public RoomID RoomId { get; private set;}

        public bool Federated {get; private set;}
        private Dictionary<EventID, PDUEvent> eventCache;
        public RoomGraph(RoomID roomId, bool federated) {
            RoomId = roomId;
            eventCache = new Dictionary<EventID, PDUEvent>();
            Federated = federated;
        }

        public void InsertClientEvent(ClientEvent clientEvent) {
            // Validate the event.
            if(clientEvent.RoomId != RoomId){
                throw new Exception("RoomId does not match.");
            }
            if(String.IsNullOrWhiteSpace(clientEvent.Type)){
                throw new Exception("A type must be given.");
            }
            // Get the tip of the graph.
            // Transform the client event into a room event.

            return false;
        }

        private PDUEvent getEvent(EventID eventId){
            if(eventCache.ContainsKey(eventId)){
                return eventCache[eventId];
            }
            //TODO: Fetch from DB.
            DbQueue.Request();
            DbQueue.ListenForResponse();
            if(Federated) {
                //TODO: Fetch from Federation handler.
                //FederationRequest.Request();
                //FederationRequest.ListenForResponse();
            } else{
                return null;
            }
            

        }
        private bool isEventAuthorised(PDUEvent event)
        {
            return false;
        }
    }
}