using System;
using System.Threading;
using HalfShot.MagnetHS.ClientServerAPIService.Controllers;
using System.Net;
using System.Collections.Generic;
namespace HalfShot.MagnetHS.ClientServerAPIService
{
    class ClientServerAPI
    {
        public static Configuration Config;
        public static readonly string[] SUPPORTED_VERSIONS = new string[] { "r0.0.1", "r0.1.0", "r0.2.0" };
        static void Main(string[] args)
        {
            Logger.StartLogger();
            Logger.Info("Started ClientServerAPI");
            if(args.Length > 0)
            {
                Config = CommonStructures.Meta.ServiceConfiguration.FromYAMLFile<Configuration>(args[0]);
            } else
            {
                Logger.Warn("No config file supplied, using defaults.");
                Config = new Configuration();
            }

            HttpListener listener = new HttpListener();
            var controllers = new List<RestController>();
            controllers.Add(new VersionsController());
            controllers.Add(new ProfileController());
            controllers.Add(new RoomController());
            controllers.Add(new CreateRoomController());
            controllers.Add(new LoginController());
            controllers.Add(new UserController());
            controllers.ForEach((controller) =>
            {
                Logger.Info($"Registering endpoints for {controller.GetType().Name}");
                controller.RegisterEndpoint(listener, Config.RootPath);
            });
            listener.Start();
            Uri UriPath = new Uri(Config.RootPath, UriKind.Absolute);
            while(true)
            {
                var context = listener.GetContext();
                Logger.Debug($"{context.Request.HttpMethod} {context.Request.Url.AbsolutePath}");
                string path = context.Request.Url.AbsolutePath.Replace(UriPath.AbsolutePath, "");
                foreach (var controller in controllers)
                {
                    if(controller.HandleRequest(context, path))
                    {
                        Logger.Debug($"Handled by {controller.GetType().Name}");
                        break;
                    }
                }
            }
        }
    }
}
