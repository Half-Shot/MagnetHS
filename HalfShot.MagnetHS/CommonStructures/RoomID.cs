using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures
{
    public class RoomID : MatrixCommonId
    {
        public RoomID(string roomId) : base('!', roomId)
        {

        }

        public static RoomID Generate(string domain) {
            return new RoomID($"!{MatrixCommonId.GenerateLocalpart()}:{domain}");
        }
    }
}
