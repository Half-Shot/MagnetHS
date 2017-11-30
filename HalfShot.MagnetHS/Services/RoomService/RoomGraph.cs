using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Room;
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
        IRoomEventStore eventStore;
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
            memberStates = new Dictionary<UserID, EMembership>();
            Federated = federated;
        }

        public void SetEventSource(IRoomEventStore source) {
            eventStore = source;
        }

        public void InsertEvents(params PDUEvent[] pduEvents)
        {
            // Validate the event.
            // Get the tip of the graph.
            List<PDUEvent> events = pduEvents.ToList();
            //TODO: Get tip
            EventID prev_event_id = null;
            foreach (var pduEvent in pduEvents)
            {
                if (prev_event_id != null)
                {
                    pduEvent.PreviousEvents.Add(new EventHash()
                    {
                        EventId = prev_event_id,
                        SHA256 = ""
                    });
                }
                
                if (pduEvent.RoomId != RoomId)
                {
                    throw new Exception("RoomId does not match.");
                }
                if (String.IsNullOrWhiteSpace(pduEvent.Type))
                {
                    throw new Exception("A type must be given.");
                }
                if (pduEvent.EventId == null)
                {
                    throw new Exception("An eventid must be given.");
                }
                if(!IsEventAuthorized(pduEvent)) {
                    throw new Exception("Event is not authorised");
                }
                prev_event_id = pduEvent.EventId;
            }

            try
            {
                eventStore.PutEvent(pduEvents);
                foreach (var ev in pduEvents)
                {
                    eventCache.Add(ev.EventId, ev);
                    Depth++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            PDUEvent ev_joinrules = eventStore.GetStateEvent("m.room.join_rules");
            if (ev_joinrules != null)
            {
                JoinRule = (ev_joinrules.Content as RoomJoinRules).JoinRule;
            }
            else
            {
                JoinRule = EJoinRule.Private;
            }
            
            PDUEvent ev_powerlevels = eventStore.GetStateEvent("m.room.power_levels");
            if (ev_powerlevels != null)
            {
                PowerLevels = ev_powerlevels.Content as RoomPowerLevels;
            }
            else
            {
                PowerLevels = new RoomPowerLevels();
            }
            
        }

        private PDUEvent getEvent(EventID eventId){
            PDUEvent ev;

            if(eventCache.ContainsKey(eventId)){
                return eventCache[eventId];
            }
            try {
                ev = eventStore.GetEvent(eventId).FirstOrDefault();
            } catch (Exception ex) {
                throw new Exception("Exception when trying to retrieve from the event source.", ex);
            }
            
            if(ev != null) {
                eventCache.Add(ev.EventId, ev);
                return ev;
            }

            if (Federated) {
                //TODO: Fetch from Federation handler.
                //FederationRequest.Request();
                //FederationRequest.ListenForResponse();
            }
            return null;
        }

        public bool IsEventAuthorized(PDUEvent ev)
        {
            if(ev.Type == "m.room.create")
            {
                return ev.Depth == 0;
            } else if (ev.Type == "m.room.member") {
                return IsMemberEventAuthorized(ev);
            } else if (GetMembership(ev.Sender) != EMembership.Join) {
                return false;
            } else if(
                PowerLevels.Events.GetValueOrDefault(ev.Type, PowerLevels.EventsDefault) > GetPowerlevel(ev.Sender)
                ){
                return false;
            } else if (ev.Type == "m.room.power_levels") {
                //TODO: Find previous events -- allow if none exist.
                int senderPowerLevel = GetPowerlevel(ev.Sender);
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
            var state = memberStates.GetValueOrDefault(user, EMembership.None);
            if (state == EMembership.None) { // DB Store
                PDUEvent ev = eventStore.GetStateEvent("m.room.member", user.ToString());
                if(ev != null) {
                    eventCache.Add(ev.EventId, ev);
                }
                //TODO: Decode
            }
            if (state == EMembership.None) { // Federation

            }
            return state;
        }

        private int GetPowerlevel(UserID user) {
            return PowerLevels.Users.GetValueOrDefault(user, PowerLevels.UsersDefault);
            //TODO: If none, fetch event based on critera.
        }

        private bool IsMemberEventAuthorized(PDUEvent ev) {
            RoomMember member = ev.Content as RoomMember;
            var senderMembership = GetMembership(ev.Sender);
            if(member.Membership == EMembership.Join) {
                if(ev.PreviousEvents.Count == 1) {
                    var previousEv = getEvent(ev.PreviousEvents[0].EventId);
                    if (previousEv?.Type == "m.room.create") {
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