using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue.ZMQ;

namespace HalfShot.MagnetHS.MessageQueue
{
    public class MQConnector
    {
        public static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
        static Dictionary<EMQService, Type> serviceTypes = new Dictionary<EMQService, Type>();
        static Type defaultType = typeof(ZeroMessageQueue);


        public static void OverrideTypeForService<T>(EMQService service) where T: IMessageQueue
        {
            serviceTypes[service] = typeof(T);
        }

        public static IMessageQueue GetResponder(EMQService service)
        {
            IMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.Setup(getConnStrForService(service), EMQType.Respond);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetRequester(EMQService service)
        {
            IMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.Setup(getConnStrForService(service), EMQType.Request);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetPusher(EMQService service)
        {
            IMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.Setup(getConnStrForService(service), EMQType.Push);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetPuller(EMQService service)
        {
            IMessageQueue msgQueue = new ZeroMessageQueue();
            msgQueue.Setup(getConnStrForService(service), EMQType.Pull);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
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

        private static Type GetTypeForService(EMQService service)
        {
            if (serviceTypes.ContainsKey(service))
            {
                return serviceTypes[service];
            }
            return defaultType;
        } 
    }
}
