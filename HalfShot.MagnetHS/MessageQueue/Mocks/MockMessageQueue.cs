﻿using System;
namespace HalfShot.MagnetHS.MessageQueue.Mocks
{
    public class MockMessageQueue : IMessageQueue
    {
        public TimeSpan RecieveTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MQRequest ListenForRequest()
        {
            throw new NotImplementedException();
        }

        public MQResponse ListenForResponse()
        {
            throw new NotImplementedException();
        }

        public void Request(MQRequest request)
        {
            throw new NotImplementedException();
        }

        public void Respond(MQResponse request)
        {
            throw new NotImplementedException();
        }

        public void Setup(EMQService service, EMQType mqType)
        {
            // We do nothing here.
        }

        public void Dispose()
        {
            // Do nothing.
        }
    }
}
