using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures.Enums;

namespace HalfShot.MagnetHS.CommonStructures.Events.Content
{
    [Serializable]
    public class RoomJoinRules : IEventContent
    {
        public EJoinRule JoinRule { get; set; }
        
        public void FromJsonContent(string json)
        {
            throw new NotImplementedException();
        }

        public object ToCanonicalObject()
        {
            string joinRuleStr = null;
            switch (JoinRule)
            {
                    case EJoinRule.Invite:
                        joinRuleStr = "invite";
                        break;
                    case EJoinRule.Knock:
                        joinRuleStr = "knock";
                        break;
                    case EJoinRule.Private:
                        joinRuleStr = "private";
                        break;
                    case EJoinRule.Public:
                        joinRuleStr = "public";
                        break;
                    case EJoinRule.Unknown:
                        joinRuleStr = "";
                        break;
            }
            return new
            {
                join_rule = joinRuleStr
            };
        }
    }
}
