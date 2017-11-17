using System;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Requests
{
    [Serializable]
    class LoginRequest
    {
        public string type { get; set; }
        public string user { get; set; }
        public string medium { get; set; }
        public string address { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public string device_id { get; set; }
        public string initial_device_display_name { get; set; }

        public ELoginType Type { get
            {
                switch (type)
                {
                    case "m.login.password":
                        return ELoginType.Password;
                    case "m.login.token":
                        return ELoginType.Token;
                    default:
                        return ELoginType.Unknown;
                }
            }
        }
    }
}
