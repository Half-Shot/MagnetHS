using System;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Requests
{
    [Serializable]
    public class CreateRoomRequest
    {
        public string name { get; set; } = null;
        public string topic { get; set; } = null;
    }
}