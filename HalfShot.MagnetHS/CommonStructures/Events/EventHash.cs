using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    [Serializable]
    public class EventHash
    {
        public EventID EventId;
        public string SHA256;
    }
}