using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using HalfShot.MagnetHS.ClientServerAPIService.Responses;
namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("versions")]
    class VersionsController : RestController
    {
        [RestEndPoint("GET")]
        public void GetProfile(RestContext context)
        {
            var versions = new VersionsResponse() { versions = ClientServerAPI.SUPPORTED_VERSIONS };
            using(var stream = context.DataTransformer.ToStream(versions)) {
                stream.CopyTo(context.HttpContext.Response.OutputStream);
            }
            context.HttpContext.Response.OutputStream.Close();
        }
    }
}
