using System;
using System.Threading;
using HalfShot.MagnetHS.ClientServerAPIService.Controllers;
using System.Net;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.ClientServerAPIService
{
    class ClientServerAPI
    {
        const string rootPath = "http://localhost:8448/_matrix/client";
        public static readonly string[] SUPPORTED_VERSIONS = new string[] { "r0.0.1", "r0.1.0", "r0.2.0" };
        static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            var controllers = new List<RestController>();
            controllers.Add(new VersionsController());
            controllers.Add(new ProfileController());
            controllers.ForEach((controller) => controller.RegisterEndpoint(listener, rootPath));
            listener.Start();
            while(true)
            {
                var context = listener.GetContext();
                string path = context.Request.Url.AbsolutePath.Replace("/_matrix/client", "");
                foreach (var controller in controllers)
                {
                    if(controller.HandleRequest(context, path))
                    {
                        break;
                    }
                }
            }
        }
    }
}
