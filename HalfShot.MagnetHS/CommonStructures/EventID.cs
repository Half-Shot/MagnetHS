using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures
{
    public class EventID : MatrixCommonId
    {
        public EventID(string eventId) : base('$', eventId)
        {

        }
    }
}
