using System.Collections.Generic;
using HalfShot.MagnetHS.CommonStructures;

namespace HalfShot.MagnetHS.RoomService
{
    public class RoomPowerLevels
    {
        public int Ban {get; set;} = 50;
        public int EventsDefault {get; set;} = 0;
        public int UsersDefault {get; set;} = 0;
        public int Invite {get; set;} = 0;
        public int Kick {get;set;} = 50;
        public int Redact {get;set;} = 50;
        public int StateDefault {get;set;} = 50;
        public Dictionary<string,int> Events {get;} = new Dictionary<string, int>();
        public Dictionary<UserID,int> Users {get;} = new Dictionary<UserID, int>();
    }
}