using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using RabbitMQ.Client ;
using HalfShot.MagnetHS.MessageQueue;
using RabbitMQ.Client.Framing.Impl;

namespace MagnetHS.MessageQueue.RabbitMQ
{
    public class RabbitMessageQueue : IMessageQueue
    {
        private const string EXCH_NAME = "Half-Shot.MagnetHS";
        private IConnection connection;
        private IModel model;
        public void Dispose()
        {
            connection.Dispose();
            throw new NotImplementedException();
        }

        public TimeSpan RecieveTimeout { get; set; }
        public void Request(MQRequest request)
        {
            
        }

        public void Respond(MQResponse request)
        {
            throw new NotImplementedException();
        }

        public MQRequest ListenForRequest()
        {
            throw new NotImplementedException();
        }

        public MQResponse ListenForResponse()
        {
            throw new NotImplementedException();
        }

        public void Setup(EMQService service, EMQType mqType, string routingKey = "")
        {
            string queueName = Enum.GetName(typeof(EMQService), service);
            connection = new ConnectionFactory().CreateConnection();
            model = connection.CreateModel();
            model.ExchangeDeclare(
                EXCH_NAME,
                ExchangeType.Direct,
                autoDelete: false,
                durable: false,
                arguments: null
                );
            switch (mqType)
            {
                case EMQType.Request:
                    model.QueueDeclare(
                        queueName,
                        durable: QueueIsDurable(service),
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    model.QueueBind(queueName, EXCH_NAME, routingKey, null);
                    break;
                case EMQType.Respond:
                    break;
                //case EMQType.Push:
                //    break;
                //case EMQType.Pull:
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mqType), mqType, null);
            }
        }

        private bool QueueIsDurable(EMQService service)
        {
            
        }
    }
}
