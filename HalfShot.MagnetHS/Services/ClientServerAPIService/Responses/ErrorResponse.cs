using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Responses
{
    [Serializable]
    class ErrorResponse
    {
        public string errcode { get; set; }
        public string error { get; set; }
        public string debug { get; set; }
    }
}
