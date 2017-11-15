using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Requests
{
    public enum LogRequestLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Critical
    }

    [Serializable]
    public class LogRequest : MessageQueue.MQRequest
    {
        public string Instance { get; set; }
        public string Service { get; set; }
        public string Message { get; set; }
        public int? RequestN { get; set; } = null;
        public DateTime Time { get; set; } = DateTime.Now;
        public LogRequestLevel Level { get; set; } = LogRequestLevel.Info;
    }
}
