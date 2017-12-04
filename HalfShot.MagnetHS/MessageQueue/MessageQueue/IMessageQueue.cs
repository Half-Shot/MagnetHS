using System;

namespace HalfShot.MagnetHS.MessageQueue
{
    public interface IMessageQueue : IDisposable
    {
        TimeSpan RecieveTimeout { get; set; }
        void Request(MQRequest request);
        void Respond(MQResponse request);
        MQRequest ListenForRequest();
        MQResponse ListenForResponse();
        void Setup(string connectionString, EMQType mqType);
    }
}
