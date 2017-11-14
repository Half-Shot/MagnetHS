using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    public class BaseEvent
    {
        private static JSONDataTransformer jsonTransformer = new JSONDataTransformer();
        public TimeSpan Age { get; set; }
        public object Content { get; set; }
        public string JsonContent { get
            {
                return jsonTransformer.ConvertToString(Content);
            }
        }
        public EventID EventId { get; set; }
        public DateTime OriginServerTs { get; set; }
        public UserID Sender { get; set; }
        public string StateKey { get; set; }
        public string Type { get; set; }
    }
}
