using System;
using System.Collections.Generic;
using System.Text;
namespace HalfShot.MagnetHS.ClientServerAPIService
{
    internal class Configuration : CommonStructures.Meta.ServiceConfiguration
    {
        public string RootPath { get; set; } = "http://localhost:8448/_matrix/client";

        public static Configuration DefaultConfig()
        {
            return new Configuration()
            {

            };
        }
    }
}
