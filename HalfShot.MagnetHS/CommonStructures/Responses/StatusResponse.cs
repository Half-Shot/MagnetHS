using System;

namespace HalfShot.MagnetHS.CommonStructures.Responses
{
    [Serializable]
    public class StatusResponse : MessageQueue.MQResponse
    {
        public bool Succeeded { get; set; } = true;
        public bool Stubbed { get; set; } = false;
        public string ErrorCode { get; set; }
        public string Error { get; set; }

    }
}
