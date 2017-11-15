using System;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
namespace HalfShot.MagnetHS
{
    public class Logger
    {
        private static IMessageQueue LogMQ;
        private static string ServiceName;
        public static void StartLogger()
        {
            LogMQ = MQConnector.GetPusher(EMQService.Logging);
            ServiceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
        }

        public static void Debug(string message, int? requestN = null)
        {
            LogMQ.Request(new LogRequest()
            {
                Message = message,
                Level = LogRequestLevel.Debug,
                Service = ServiceName,
                RequestN = requestN,
            });
        }

        public static void Info(string message, int? requestN = null)
        {
            LogMQ.Request(new LogRequest()
            {
                Message = message,
                Level = LogRequestLevel.Info,
                Service = ServiceName,
                RequestN = requestN,
            });
        }

        public static void Warn(string message, int? requestN = null)
        {
            LogMQ.Request(new LogRequest()
            {
                Message = message,
                Level = LogRequestLevel.Warn,
                Service = ServiceName,
                RequestN = requestN,
            });
        }

        public static void Error(string message, int? requestN = null)
        {
            LogMQ.Request(new LogRequest()
            {
                Message = message,
                Level = LogRequestLevel.Error,
                Service = ServiceName,
                RequestN = requestN,
            });
        }

        public static void Critical(string message, int? requestN = null)
        {
            LogMQ.Request(new LogRequest()
            {
                Message = message,
                Level = LogRequestLevel.Critical,
                Service = ServiceName,
                RequestN = requestN,
            });
        }
    }
}
