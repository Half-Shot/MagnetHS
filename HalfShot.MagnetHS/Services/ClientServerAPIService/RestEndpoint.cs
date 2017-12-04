using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
namespace HalfShot.MagnetHS.ClientServerAPIService
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class RestEndPointAttribute : Attribute
    {

        readonly string method;
        readonly Regex endpoint;
        readonly bool authRequired;

        public string Method => method;

        public Regex Endpoint => endpoint;

        public bool AuthRequired => authRequired;

        public RestEndPointAttribute(string method, string endpoint = null, bool authRequired = false)
        {
            this.method = method;
            if (endpoint != null)
            {
                this.endpoint = new Regex(endpoint, RegexOptions.Compiled);
            }
            this.authRequired = authRequired;
        }

        public bool URLMatch(string url)
        {
            return endpoint?.IsMatch(url) ?? (String.IsNullOrWhiteSpace(url));
        }
        
    }
}
