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

        public string Method
        {
            get { return method; }
        }

        public Regex Endpoint
        {
            get { return endpoint; }
        }

        public bool AuthRequired
        {
            get { return authRequired; }
        }
        
        public RestEndPointAttribute(string method)
        {
            this.method = method;
            this.endpoint = null;
        }

        public RestEndPointAttribute(string method, string endpoint, bool authRequired = false)
        {
            this.method = method;
            this.endpoint = new Regex(endpoint, RegexOptions.Compiled);
            this.authRequired = authRequired;
        }

        public bool URLMatch(string url)
        {
            return endpoint != null ? (endpoint.IsMatch(url)) : (String.IsNullOrWhiteSpace(url));
        }
        
    }
}
