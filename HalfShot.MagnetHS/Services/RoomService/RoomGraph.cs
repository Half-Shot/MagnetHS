using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Events;
using HalfShot.MagnetHS.CommonStructures.Events.Content;
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
        public RoomPowerLevels PowerLevels {get; private set;}
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

        public bool IsEventAuthorized(PDUEvent ev)
        {
            int RequiredPowerLevel = PowerLevels.Events.GetValueOrDefault(ev.Type, PowerLevels.EventsDefault);
            if(ev.Type == "m.room.create")
            {
                return ev.Depth == 0;
            } else if (ev.Type == "m.room.member") {
                return IsMemberEventAuthorized(ev);
            } else if (GetMembership(ev.Sender) != EMembership.Join) {
                return false;
            } else if(RequiredPowerLevel > GetPowerlevel(ev.Sender)){
                return false;
            } else if (ev.Type == "m.room.power_levels") {
                //TODO: Find previous events -- allow if none exist.
                var senderPowerLevel = GetPowerlevel(ev.Sender);
                if(PowerLevels.Ban > senderPowerLevel ||
                   PowerLevels.EventsDefault > senderPowerLevel ||
                   PowerLevels.Invite > senderPowerLevel ||
                   PowerLevels.Kick > senderPowerLevel ||
                   PowerLevels.Redact > senderPowerLevel ||
                   PowerLevels.StateDefault > senderPowerLevel ||
                   PowerLevels.Events.Values.Any((pl) => pl > senderPowerLevel) ||
                   PowerLevels.Users.Values.Any((pl) => pl > senderPowerLevel)) {
                       return false;
                   }
                //Todo, do the same check for the current power level.
                return true;
            } else if (ev.Type == "m.room.redaction") {
                if(PowerLevels.Redact <= GetPowerlevel(ev.Sender)) {
                    return true;
                }
                var redactEvent = getEvent(ev.Redacts);
                if(redactEvent.Sender == ev.Sender){
                    return true;
                }
                return false;
            }
            return true;
        }

        private EMembership GetMembership(UserID user) {
            return memberStates.GetValueOrDefault(user, EMembership.None);
        }

        private int GetPowerlevel(UserID user) {
            return PowerLevels.Users.GetValueOrDefault(user, PowerLevels.UsersDefault);
        }

        private bool IsMemberEventAuthorized(PDUEvent ev) {
            RoomMember member = ev.Content as RoomMember;
            var senderMembership = GetMembership(ev.Sender);
            if(member.Membership == EMembership.Join) {
                if(ev.PreviousEvents.Count == 1) {
                    var previousEv = getEvent(ev.PreviousEvents[0].EventId);
                    if (previousEv.Type == "m.room.create") {
                        if (ev.StateKey == previousEv.Sender.ToString()) {
                            return true;
                        }
                    }
                }
                if(ev.Sender.ToString() != ev.StateKey) {
                    return false;
                }
                if (senderMembership == EMembership.Join ||
                    senderMembership == EMembership.Invite) {
                    return true;
                }
                if(JoinRule == EJoinRule.Public) {
                    return true;
                }
                return false;
            } else if(member.Membership == EMembership.Invite) {
                if (senderMembership != EMembership.Join) {
                    return false;
                }
                var inviteMembership = GetMembership(new UserID(ev.StateKey));
                if(inviteMembership == EMembership.Join || inviteMembership == EMembership.Ban) {
                    return false;
                }
                if(GetPowerlevel(ev.Sender) >= PowerLevels.Invite) {
                    return true;
                }
            } else if(member.Membership == EMembership.Knock) {
                return false; //Knock not supported yet..
            } else if(member.Membership == EMembership.Ban) {
                if (senderMembership != EMembership.Join) {
                    return false;
                }
                if(GetPowerlevel(ev.Sender) >= PowerLevels.Ban &&
                    GetPowerlevel(ev.Sender) > GetPowerlevel(new UserID(ev.StateKey))
                ){
                    return true;
                }
            } else if(member.Membership == EMembership.Leave) {
                if(ev.Sender.ToString() == ev.StateKey) {
                    return senderMembership == EMembership.Join || senderMembership == EMembership.Invite;
                }
                if (senderMembership != EMembership.Join) {
                    return false;
                }
                var targetMembership = GetMembership(new UserID(ev.StateKey));
                if(targetMembership == EMembership.Ban && GetPowerlevel(ev.Sender) < PowerLevels.Ban){
                    return false;
                }
                if(GetPowerlevel(ev.Sender) >= PowerLevels.Kick &&
                    GetPowerlevel(ev.Sender) > GetPowerlevel(new UserID(ev.StateKey))
                ){
                    return true;
                }
            }
            return false;
        }
    }
}