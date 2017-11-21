using System;

namespace HalfShot.MagnetHS.CommonStructures.Responses
{
    [Serializable]
    public class StatusResponse : MessageQueue.MQResponse
    {
        public const string ServiceTimeout = "MGHS_SERVICETIMEOUT";
        public const string NotFound = "M_NOT_FOUND";

        public bool Succeeded { get; set; } = true;
        public bool Stubbed { get; set; } = false;
        public string ErrorCode { get; set; }
        public string Error { get; set; }

        public static StatusResponse StandardTimeoutResponse(string serviceName)
        {
            return new StatusResponse()
            {
                Succeeded = false,
                Stubbed = false,
                ErrorCode = ServiceTimeout,
                Error = $"Timed out while waiting for the {serviceName} service"
            };
        }
    }

}
