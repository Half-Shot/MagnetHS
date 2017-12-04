using System;
using System.Collections.Generic;
using System.Linq;
using HalfShot.MagnetHS.CommonStructures;

namespace HalfShot.MagnetHS.RoomService
{
    public class RoomPowerLevels
    {
        public RoomPowerLevels()
        {
            
        }

        public RoomPowerLevels(RoomPowerLevels oldState)
        {
            Ban = oldState.Ban;
            EventsDefault = oldState.EventsDefault;
            UsersDefault = oldState.UsersDefault;
            Invite = oldState.Invite;
            Kick = oldState.Kick;
            Redact = oldState.Redact;
            StateDefault = oldState.StateDefault;
            Events = new Dictionary<string, int>(oldState.Events);
            Users = new Dictionary<string, int>(oldState.Users);
        }
        
        public int Ban {get; set;} = 50;
        public int EventsDefault {get; set;} = 0;
        public int UsersDefault {get; set;} = 0;
        public int Invite {get; set;} = 0;
        public int Kick {get;set;} = 50;
        public int Redact {get;set;} = 50;
        public int StateDefault {get;set;} = 50;
        public Dictionary<string,int> Events {get;} = new Dictionary<string, int>();
        public Dictionary<string,int> Users {get;} = new Dictionary<string, int>();
    }
}