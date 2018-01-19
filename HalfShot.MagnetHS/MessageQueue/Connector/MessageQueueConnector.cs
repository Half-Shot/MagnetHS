using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.MessageQueue.ZMQ;
using MagnetHS.MessageQueue.RabbitMQ;

namespace HalfShot.MagnetHS.MessageQueue
{
    public static class MQConnector
    {
        private static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(10);
        static Dictionary<EMQService, Type> serviceTypes = new Dictionary<EMQService, Type>();
        static readonly Type defaultType = typeof(RabbitMessageQueue);


        public static void OverrideTypeForService<T>(EMQService service) where T: IMessageQueue
        {
            serviceTypes[service] = typeof(T);
        }

        public static IMessageQueue GetResponder(EMQService service)
        {
            IMessageQueue msgQueue = Activator.CreateInstance(getTypeForService(service)) as IMessageQueue;
            msgQueue.Setup(service, EMQType.Respond);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetRequester(EMQService service)
        {
            IMessageQueue msgQueue = Activator.CreateInstance(getTypeForService(service)) as IMessageQueue;
            msgQueue.Setup(service, EMQType.Request);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetPusher(EMQService service)
        {
            IMessageQueue msgQueue = Activator.CreateInstance(getTypeForService(service)) as IMessageQueue;
            msgQueue.Setup(service, EMQType.Push);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

        public static IMessageQueue GetPuller(EMQService service)
        {
            IMessageQueue msgQueue = Activator.CreateInstance(getTypeForService(service)) as IMessageQueue;
            msgQueue.Setup(service, EMQType.Pull);
            msgQueue.RecieveTimeout = DefaultTimeout;
            return msgQueue;
        }

//        /// <summary>
//        /// This should only be called by a routing service
//        /// </summary>
//        /// <param name="service"></param>
//        /// <returns></returns>
//        public static MessageRouter GetRouter(EMQService service)
//        {
//            MessageRouter router = new ZeroMessageRouter(service);
//            return router;
//        }


        private static Type getTypeForService(EMQService service)
        {
            if (serviceTypes.ContainsKey(service))
            {
                return serviceTypes[service];
            }
            return defaultType;
        } 
    }
}
