using HalfShot.MagnetHS.DataTransformer;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Events
{
    [Serializable]
    public abstract class BaseEvent
    {
        protected static JSONDataTransformer jsonTransformer = new JSONDataTransformer();
        private object content;
        private string json;
        public object Content {
            get {
                return content;
            }
            set {
                content = value;
                json = jsonTransformer.ConvertToString(Content);
            }
        }
        public string JsonContent { get
            {
                return json;
            }
            set
            {
                json = value;
            }
        }
        public string Type { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}
