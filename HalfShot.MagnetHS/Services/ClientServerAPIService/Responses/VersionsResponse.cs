using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Responses
{
    [Serializable]
    class VersionsResponse
    {
        public string[] versions { get; set; }
    }
}
