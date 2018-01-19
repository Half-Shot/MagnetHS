using System;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.MessageQueue.ZMQ
{
    public class ZeroMessageQueue : IMessageQueue
    {
        private NetMQSocket _socket;

        public TimeSpan RecieveTimeout { get; set; } = TimeSpan.MaxValue;
        public void Setup(EMQService service, EMQType mqType)
        {
            var connectionString = getConnStrForService(service);
            switch (mqType)
            {
                case EMQType.Request:
                    _socket = new RequestSocket();
                    _socket.Bind(connectionString);
                    break;
                case EMQType.Respond:
                    _socket = new ResponseSocket();
                    _socket.Connect(connectionString);
                    break;
                case EMQType.Push:
                    _socket = new PushSocket();
                    _socket.Connect(connectionString);
                    break;
                case EMQType.Pull:
                    _socket = new PullSocket();
                    _socket.Bind(connectionString);
                    break;
                default:
                    throw new NotSupportedException("Cannot create a queue of this type. Not supported!");
            }
        }

        public MQRequest ListenForRequest()
        {
            var frames = _socket.ReceiveMultipartBytes(2);
            return MQItem.FromBytes<MQRequest>(frames[1]);
        }

        public MQResponse ListenForResponse()
        {
            var frames = new List<byte[]>(2);
            if (!_socket.TryReceiveMultipartBytes(RecieveTimeout, ref frames, 2))
            {
                throw new TimeoutException("Timeout when waiting for a response from the MQ");
            }
            return MQItem.FromBytes<MQResponse>(frames[1]);
        }

        public void Request(MQRequest request)
        {
            Send(request);
        }

        public void Respond(MQResponse response)
        {
            Send(response);
        }

        private void Send(MQItem item)
        {
            var header = item.GetHeader();
            var data = item.GetBytes();
            _socket.SendMultipartBytes(header, data);
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }

        private static string getConnStrForService(EMQService service)
        {
            //TODO: Configurable connection strings
            switch (service)
            {
                case EMQService.User:
                    return "tcp://localhost:5555";
                case EMQService.Datastore:
                    return "tcp://localhost:5556";
                case EMQService.Room:
                    return "tcp://localhost:5557";
                case EMQService.Logging:
                    return "tcp://localhost:5558";
                default:
                    throw new InvalidOperationException("Unknown service");
            }
        }
        
    }
}
