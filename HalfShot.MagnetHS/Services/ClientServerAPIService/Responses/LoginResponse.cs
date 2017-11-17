using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Responses
{
    [Serializable]
    class LoginResponse
    {
        public string user_id;
        public string access_token;
        public string home_server;
        public string device_id;
    }
}
