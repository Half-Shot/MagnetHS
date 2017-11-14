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

        public void SetupResponder(string bindAddress)
        {
            socket = new ResponseSocket();
            socket.Bind(bindAddress);
        }

        public void SetupRequester(string connectionAddress)
        {
            socket = new RequestSocket();
            socket.Connect(connectionAddress);
        }

        public void SetupPusher(string connectionAddress)
        {
            socket = new PushSocket();
            socket.Connect(connectionAddress);
        }

        public void SetupPuller(string connectionAddress)
        {
            socket = new PullSocket();
            socket.Bind(connectionAddress);
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
