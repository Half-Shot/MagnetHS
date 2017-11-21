using System;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.MessageQueue.ZMQ
{
    public class ZeroMessageQueue : IMessageQueue, IDisposable
    {
        NetMQSocket socket;

        public TimeSpan RecieveTimeout { get; set; } = TimeSpan.MaxValue;


        public void Setup(string connectionString, EMQType mqType)
        {
            switch (mqType)
            {
                case EMQType.Request:
                    socket = new RequestSocket();
                    socket.Bind(connectionString);
                    break;
                case EMQType.Respond:
                    socket = new ResponseSocket();
                    socket.Connect(connectionString);
                    break;
                case EMQType.Push:
                    socket = new PushSocket();
                    socket.Connect(connectionString);
                    break;
                case EMQType.Pull:
                    socket = new PullSocket();
                    socket.Bind(connectionString);
                    break;
                default:
                    throw new NotSupportedException("Cannot create a queue of this type. Not supported!");
            }
        }

        public MQRequest ListenForRequest()
        {
            List<byte[]> frames = socket.ReceiveMultipartBytes(2);
            return MQItem.FromBytes<MQRequest>(frames[1]);
        }

        public MQResponse ListenForResponse()
        {
            List<byte[]> frames = new List<byte[]>(2);
            if (!socket.TryReceiveMultipartBytes(RecieveTimeout, ref frames, 2))
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
            socket.SendMultipartBytes(header, data);
        }

        public void Dispose()
        {
            if (socket != null)
            {
                socket.Dispose();
            }
        }
    }
}
