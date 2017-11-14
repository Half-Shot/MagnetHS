using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HalfShot.MagnetHS.DataTransformer;
using System.Collections.Specialized;

namespace HalfShot.MagnetHS.ClientServerAPIService
{
    public class RestContext
    {
        const ERestContentType DEFAULT_CONTENTTYPE = ERestContentType.Json;
        private static Dictionary<string, ERestContentType> ContentTypeMappings = new Dictionary<string, ERestContentType> () {
            { "application/json", ERestContentType.Json},
            { "application/x-msgpack", ERestContentType.MessagePack },
            { "application/msgpack", ERestContentType.MessagePack },
        };
        
        public HttpListenerContext HttpContext { get; private set; }

        private ERestContentType expectedType;
        public ERestContentType SuggestedContentType { get { return expectedType; } }
        public IDataTransformer DataTransformer { get; private set; }
        public NameValueCollection Parameters { get { return HttpContext.Request.QueryString; } }
        public Dictionary<string, string> PathParameters { get; private set; }

        public RestContext()
        {
            PathParameters = new Dictionary<string, string>();
        }

        public void AttachContext(HttpListenerContext context)
        {
            HttpContext = context;
            DetectExpectedType();
            SetupDataTransformer();
            SetContentType();
            DetectExpectedType();
        }

        public void SetContentType()
        {
            HttpContext.Response.ContentType = ContentTypeMappings.First((kv) => kv.Value == SuggestedContentType).Key;
        }

        
        private void DetectExpectedType()
        {
            // TODO: Match wildcard Accept options.
            // TODO: Acknowledge preferences.
            expectedType = ERestContentType.Unknown;
            foreach (var contentType in HttpContext.Request.AcceptTypes)
            {
                if(ContentTypeMappings.TryGetValue(contentType, out expectedType))
                {
                    break;
                }
            }
            if (expectedType == ERestContentType.Unknown)
            {
                expectedType = DEFAULT_CONTENTTYPE;
                //throw new RestContentTypeNotSupported($"None of the given acceptable types are supported.");
            }
        }

        public void SetupDataTransformer()
        {
            switch (expectedType)
            {
                case ERestContentType.Json:
                    DataTransformer = new JSONDataTransformer();
                    break;
                case ERestContentType.MessagePack:
                    //TODO: Add Messagepack Transformer
                    break;
            }
        }

        public void ProcessPathParameters(Regex endpointRegex, string path)
        {
            if(endpointRegex == null)
            {
                return;
            }
            var match = endpointRegex.Match(path);
            foreach (Group group in match.Groups)
            {
                PathParameters.Add(group.Name, group.Success ? group.Value : null);
            }
        }
    }
}
