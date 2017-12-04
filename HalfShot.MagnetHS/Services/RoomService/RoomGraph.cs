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
using HalfShot.MagnetHS.RoomService.Exceptions;
using System.Linq;

namespace HalfShot.MagnetHS.RoomService
{
    public class RoomGraph
    {
        //static IMessageQueue FederationRequest = Program.FederationRequest;
        IRoomEventStore eventStore;
        public RoomID RoomId { get; private set;}
        public EJoinRule JoinRule { get; private set; }
        public RoomPowerLevels PowerLevels {get; private set;}
        public bool Federated {get; private set;}
        public int Depth { get; private set; }
        private Dictionary<string, EMembership> memberStates;
        private Dictionary<string, PDUEvent> eventCache;
        private Dictionary<string, PDUEvent> tempEventCache;

        public RoomGraph(RoomID roomId, bool federated = true) {
            RoomId = roomId;
            eventCache = new Dictionary<string, PDUEvent>();
            tempEventCache = new Dictionary<string, PDUEvent>();
            memberStates = new Dictionary<string, EMembership>();
            PowerLevels = new RoomPowerLevels();
            JoinRule = EJoinRule.Unknown;
            Federated = federated;
        }

        public void SetEventSource(IRoomEventStore source) {
            eventStore = source;
        }

        public void InsertEvents(params PDUEvent[] pduEvents)
        {
            insertPduEvents(pduEvents.ToList(), ERoomGraphInsertBehaviour.ChainEvents);
        }

        private void insertPduEvents(List<PDUEvent> eventList, ERoomGraphInsertBehaviour behaviour)
        {
            // Validate the event.
            // Get the tip of the graph.
            string unauthorizedReason = null;
            
            // Store a copy of the old room state!
            EJoinRule oldJoinRule = JoinRule;
            RoomPowerLevels oldPowerLevels = new RoomPowerLevels(PowerLevels);
            int oldDepth = Depth;
            Dictionary<string, EMembership> oldMemberStates = new Dictionary<string, EMembership>(memberStates);
            foreach (var pduEvent in eventList)
            {
                if (behaviour.HasFlag(ERoomGraphInsertBehaviour.ChainEvents) &&
                    tempEventCache.Count > 0l &&
                    pduEvent.PreviousEvents.Count == 0)
                {
                    pduEvent.PreviousEvents.Add(new EventHash()
                    {
                        EventId = new EventID(tempEventCache.Last().Key),
                        SHA256 = tempEventCache.Last().Value.CalculateHash(EEventHashType.Sha256)
                    });
                }

                try
                {
                    if (pduEvent.RoomId != RoomId)
                    {
                        throw new GraphInvalidRoomIdException(this, pduEvent);
                    }
                    if (String.IsNullOrWhiteSpace(pduEvent.Type))
                    {
                        throw new GraphInvalidTypeException(this, pduEvent);
                    }
                    if (pduEvent.Sender == null)
                    {
                        throw new GraphInvalidSenderException(this, pduEvent);
                    }
                    if (pduEvent.EventId == null)
                    {
                        throw new GraphInvalidEventIdException(this, pduEvent);
                    }
                    if(!IsEventAuthorized(pduEvent, out unauthorizedReason)) {
                        throw new GraphUnauthorizedException(this, pduEvent, unauthorizedReason);
                    }
                }
                catch (GraphInsertionException e)
                {
                    // Reset state.
                    JoinRule = oldJoinRule;
                    PowerLevels = oldPowerLevels;
                    Depth = Depth;
                    memberStates = oldMemberStates;
                    tempEventCache.Clear();
                    throw;
                }

                // Update Room State.
                updateRoomState(pduEvent);
                tempEventCache.Add(pduEvent.EventId.ToString(), pduEvent);
            }
            tempEventCache.Clear();
            try
            {
                eventStore.PutEvent(eventList.ToArray());
                foreach (var ev in eventList)
                {
                    eventCache.Add(ev.EventId.ToString(), ev);
                    Depth++;
                }
            }
            catch (Exception e)
            {
                //TODO: Exception during storage. Revert graph in DB.
                //Revert room state.
                JoinRule = oldJoinRule;
                PowerLevels = oldPowerLevels;
                Depth = Depth;
                memberStates = oldMemberStates;
                throw e;
            }
            //Federate events out!
        }


        private void updateRoomState(PDUEvent stateEvent)
        {
            //This event should already have been cleared for auth!
            //TODO: Update for more test cases!
            switch (stateEvent.Type)
            {
               case "m.room.member":
                   RoomMember memberState = stateEvent.Content as RoomMember;
                   memberStates[stateEvent.StateKey] = memberState.Membership;
                   break;
               case "m.room.join_rules":
                   RoomJoinRules joinRules = stateEvent.Content as RoomJoinRules;
                   JoinRule = joinRules.JoinRule;
                   break;
               case "m.room.power_levels":
                   RoomPowerLevels powerLevels = stateEvent.Content as RoomPowerLevels;
                   PowerLevels = powerLevels;
                   break;
                   
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
            PDUEvent ev_joinrules = eventStore.GetStateEvent(RoomId, "m.room.join_rules");
            if (ev_joinrules != null)
            {
                JoinRule = (ev_joinrules.Content as RoomJoinRules).JoinRule;
            }
            else
            {
                JoinRule = EJoinRule.Private;
            }
            
            PDUEvent ev_powerlevels = eventStore.GetStateEvent(RoomId, "m.room.power_levels");
            if (ev_powerlevels != null)
            {
                PowerLevels = ev_powerlevels.Content as RoomPowerLevels;
            }
            else
            {
                PowerLevels = new RoomPowerLevels();
            }
            
        }

        private bool StateEventExists(string type, string stateKey = null)
        {
            if (eventCache.Values.FirstOrDefault(
                    (ev) => ev.Type == type && (stateKey == null || ev.StateKey == stateKey)
                ) == null)
            {
                if (eventStore.GetStateEvent(RoomId, type, stateKey) == null)
                {
                    return false;
                }
            }
            return true;

        }

        private PDUEvent getEvent(EventID eventId){
            PDUEvent ev;
            string _eventID = eventId.ToString();
            if(eventCache.ContainsKey(_eventID)){
                return eventCache[_eventID];
            }
            if(tempEventCache.ContainsKey(_eventID)){
                return tempEventCache[_eventID];
            }
            try {
                ev = eventStore.GetEvent(RoomId,eventId).FirstOrDefault();
            } catch (Exception ex) {
                throw new Exception("Exception when trying to retrieve from the event source.", ex);
            }
            
            if(ev != null) {
                eventCache.Add(_eventID, ev);
                return ev;
            }

            if (Federated) {
                //TODO: Fetch from Federation handler.
                //FederationRequest.Request();
                //FederationRequest.ListenForResponse();
            }
            return null;
        }

        public bool IsEventAuthorized(PDUEvent ev, out string reason)
        {
            if(ev.Type == "m.room.create")
            {
                if (ev.Depth != 0)
                {
                    reason = "m.room.create event must be at depth 0";
                    return false;
                }
                reason = "";
                return true;
            } else if (ev.Type == "m.room.member") {
                return IsMemberEventAuthorized(ev, out reason);
            } else if (GetMembership(ev.Sender) != EMembership.Join)
            {
                reason = "Sender's membership is not joined";
                return false;
            } else if(
                PowerLevels.Events.GetValueOrDefault(ev.Type, PowerLevels.EventsDefault) > GetPowerlevel(ev.Sender)
                )
            {
                reason = "Sender's powerlevel was too low for even type";
                return false;
            } else if (ev.Type == "m.room.power_levels") {
                //TODO: Find previous events -- allow if none exist.
                if (!StateEventExists("m.room.power_levels"))
                {
                    reason = "";
                    return true;
                }
                int senderPowerLevel = GetPowerlevel(ev.Sender);
                if(PowerLevels.Ban > senderPowerLevel ||
                   PowerLevels.EventsDefault > senderPowerLevel ||
                   PowerLevels.Invite > senderPowerLevel ||
                   PowerLevels.Kick > senderPowerLevel ||
                   PowerLevels.Redact > senderPowerLevel ||
                   PowerLevels.StateDefault > senderPowerLevel ||
                   PowerLevels.Events.Values.Any((pl) => pl > senderPowerLevel) ||
                   PowerLevels.Users.Values.Any((pl) => pl > senderPowerLevel))
                {
                    reason = "Sender is not allowed to adjust power levels";
                       return false;
                   }
                //Todo, do the same check for the current power level.
                reason = "";
                return true;
            } else if (ev.Type == "m.room.redaction")
            {
                reason = "";
                if(PowerLevels.Redact <= GetPowerlevel(ev.Sender)) {
                    return true;
                }
                var redactEvent = getEvent(ev.Redacts);
                if (redactEvent.Sender == ev.Sender)
                {
                    return true;
                }
                reason = "Sender does not have the power to redact this event";
                return false;
            }
            reason = "";
            return true;
        }

        private EMembership GetMembership(UserID user) {
            var state = memberStates.GetValueOrDefault(user.ToString(), EMembership.None);
            if (state == EMembership.None) { // DB Store
                PDUEvent ev = eventStore.GetStateEvent(RoomId, "m.room.member", user.ToString());
                if(ev != null) {
                    eventCache.Add(ev.EventId.ToString(), ev);
                    updateRoomState(ev);
                }
                //TODO: Decode
            }
            if (state == EMembership.None) { // Federation

            }
            return state;
        }

        private int GetPowerlevel(UserID user) {
            return PowerLevels.Users.GetValueOrDefault(user.ToString(), PowerLevels.UsersDefault);
            //TODO: If none, fetch event based on critera.
        }

        private bool IsMemberEventAuthorized(PDUEvent ev, out string reason) {
            RoomMember member = ev.Content as RoomMember;
            var senderMembership = GetMembership(ev.Sender);
            reason = "Unknown membership state!";
            if(member.Membership == EMembership.Join) {
                if(ev.PreviousEvents.Count == 1) {
                    var previousEv = getEvent(ev.PreviousEvents[0].EventId);
                    if (previousEv?.Type == "m.room.create") {
                        if (ev.StateKey == previousEv.Sender.ToString())
                        {
                            reason = "";
                            return true;
                        }
                    }
                }
                if(ev.Sender.ToString() != ev.StateKey)
                {
                    reason = "Sender tried to adjust another users m.room.member event";
                    return false;
                }
                if (senderMembership == EMembership.Join ||
                    senderMembership == EMembership.Invite) {
                    reason = "";
                    return true;
                }
                if(JoinRule == EJoinRule.Public && senderMembership != EMembership.Ban) {
                    reason = "";
                    return true;
                }
                reason = "Sender is not allowed to join this room.";
            } else if(member.Membership == EMembership.Invite) {
                if (senderMembership != EMembership.Join) {
                    reason = "Sender cannot invite others, because they have not joined the room.";
                    return false;
                }
                var inviteMembership = GetMembership(new UserID(ev.StateKey));
                if(inviteMembership == EMembership.Join || inviteMembership == EMembership.Ban) {
                    reason = "Sender cannot invite this user, because the user is joined or banned.";
                    return false;
                }
                if (GetPowerlevel(ev.Sender) >= PowerLevels.Invite)
                {
                    reason = "";
                    return true;
                }
                reason = "Sender cannot invite this user, because they do not have the required power.";
            } else if(member.Membership == EMembership.Knock) {
                reason = "Knocking is not supported yet.";
                return false; //Knock not supported yet..
            } else if(member.Membership == EMembership.Ban) {
                if (senderMembership != EMembership.Join) {
                    reason = "Sender cannot ban others, they have not joined the room.";
                    return false;
                }
                if (GetPowerlevel(ev.Sender) >= PowerLevels.Ban &&
                    GetPowerlevel(ev.Sender) > GetPowerlevel(new UserID(ev.StateKey))
                )
                {
                    reason = "";
                    return true;
                }
                reason = "Sender cannot ban others, they do not have the required powerlevel to ban this user.";
            } else if(member.Membership == EMembership.Leave) {
                if(ev.Sender.ToString() == ev.StateKey) {
                    if (senderMembership == EMembership.Join || senderMembership == EMembership.Invite)
                    {
                        reason = "";
                        return true;
                    }
                    reason = "Sender cannot leave/reject room, because they are not joined/invited.";
                    return false;
                }
                if (senderMembership != EMembership.Join) {
                    reason = "Sender cannot leave, they have not joined the room.";
                    return false;
                }
                var targetMembership = GetMembership(new UserID(ev.StateKey));
                if(targetMembership == EMembership.Ban && GetPowerlevel(ev.Sender) < PowerLevels.Ban){
                    reason = "Sender cannot unban a user, because they do not have the required power.";
                    return false;
                }
                if(GetPowerlevel(ev.Sender) >= PowerLevels.Kick &&
                    GetPowerlevel(ev.Sender) > GetPowerlevel(new UserID(ev.StateKey))
                ){
                    reason = "";
                    return true;
                }
                reason = "Sender cannot kick this user, because they do not have the required power.";
            }
            return false;
        }
    }
}