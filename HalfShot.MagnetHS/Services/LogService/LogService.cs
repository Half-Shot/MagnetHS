using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using System;
using System.Collections.Generic;
using NLog.Config;
using NLog;
namespace HalfShot.MagnetHS.LogService
{
    class LogService
    {
        static IMessageQueue IncomingQueue;
        static Logger logger;
        static void Main(string[] args)
        {
            IncomingQueue = MQConnector.GetPuller(EMQService.Logging);
            SetupLogging();
            logger = LogManager.GetLogger("NOSERVICE");
            HandleLogRequest(new LogRequest() { Service = "LogService", Message = "-----------------------" });
            while (true)
            {
                MQRequest request = IncomingQueue.ListenForRequest();
                switch (request.GetType().Name)
                {
                    case "LogRequest":
                        HandleLogRequest(request as LogRequest);
                        break;
                    default:
                        break;
                }
            }
        }

        static void HandleLogRequest(LogRequest request)
        {
            var ev = new LogEventInfo()
            {
                LoggerName = request.Service,
                Message = request.Message,
                TimeStamp = request.Time,
                Level = GetLogLevel(request.Level),
            };
            ev.Properties.Add("instance",request.Instance);
            ev.Properties.Add("requestn", request.RequestN);
            logger.Log(ev);
        }

        static LogLevel GetLogLevel(LogRequestLevel requestLevel)
        {
            switch (requestLevel)
            {
                case LogRequestLevel.Debug:
                    return LogLevel.Debug;
                case LogRequestLevel.Info:
                    return LogLevel.Info;
                case LogRequestLevel.Warn:
                    return LogLevel.Warn;
                case LogRequestLevel.Error:
                    return LogLevel.Error;
                case LogRequestLevel.Critical:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Info;
            }
        }

        static void SetupLogging()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            var fileTarget = new NLog.Targets.FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = "${basedir}/file.txt";
            fileTarget.Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} [${level}] [${event-properties:item=requestn}] ${logger}[${event-properties:item=instance}] ${message}";

            // Step 4. Define rules
            var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }
}
