using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures
{
    public class RoomID : MatrixCommonId
    {
        const int LocalpartLength = 18;
        public RoomID(string roomId) : base('!', roomId)
        {

        }

        public static RoomID GenerateRoomId(string domain)
        {
            Random random = new Random();
            string id = "";
            while(id.Length < LocalpartLength)
            {
                int c = 91;
                while(c >= 91 && c <= 96)
                {
                    c = random.Next(65, 122);
                }
                id += (char)c;
            }
            return new RoomID($"!{id}:{domain}");
        }
    }
}
