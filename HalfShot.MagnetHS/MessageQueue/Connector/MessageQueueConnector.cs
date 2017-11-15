using System;
using HalfShot.MagnetHS.MessageQueue.ZMQ;

namespace HalfShot.MagnetHS.MessageQueue
{
    public enum EMQService
    {
        User,
        Datastore,
        Room,
        Logging
    }

    public class MQConnector
    {
        public static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
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

        public static IMessageQueue GetResponder(EMQService service)
        {
            ZeroMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.SetupResponder(getConnStrForService(service));
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetRequester(EMQService service)
        {
            ZeroMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.SetupRequester(getConnStrForService(service));
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetPusher(EMQService service)
        {
            ZeroMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.SetupPusher(getConnStrForService(service));
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetPuller(EMQService service)
        {
            ZeroMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.SetupPuller(getConnStrForService(service));
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }
    }
}
