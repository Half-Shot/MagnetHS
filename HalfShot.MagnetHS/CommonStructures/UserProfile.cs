using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures
{
    [Serializable]
    public class UserProfile
    {
        public UserID UserId { get; set; }
        public Dictionary<string, string> Profile { get; set; } = new Dictionary<string, string>();
    }
}
