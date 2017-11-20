using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    public abstract class BaseEvent
    {
        protected static JSONDataTransformer jsonTransformer = new JSONDataTransformer();
        public object Content { get; set; }
        public string JsonContent { get
            {
                return jsonTransformer.ConvertToString(Content);
            }
        }
        public string Type { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}
