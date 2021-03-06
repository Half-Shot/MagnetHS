﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HalfShot.MagnetHS.DataTransformer;
using System.Collections.Specialized;
using HalfShot.MagnetHS.CommonStructures;

namespace HalfShot.MagnetHS.ClientServerAPIService
{
    public class RestContext
    {
        public static int RequestCounter = 0;

        const ERestContentType DEFAULT_CONTENTTYPE = ERestContentType.Json;
        private static Dictionary<string, ERestContentType> ContentTypeMappings = new Dictionary<string, ERestContentType> () {
            { "application/json", ERestContentType.Json},
            { "application/x-msgpack", ERestContentType.MessagePack },
            { "application/msgpack", ERestContentType.MessagePack },
        };
        
        public HttpListenerContext HttpContext { get; private set; }

        private ERestContentType expectedType;
        public ERestContentType SuggestedContentType => expectedType;
        public IDataTransformer DataTransformer { get; private set; }
        public NameValueCollection Parameters => HttpContext.Request.QueryString;
        public Dictionary<string, string> PathParameters { get; }
        public int RequestId { get; }
        public UserID AuthenticatedUser { get; set; }

        public RestContext()
        {
            PathParameters = new Dictionary<string, string>();
            if(RequestCounter == int.MaxValue)
            {
                RequestCounter = 0;
            }
            RequestId = RequestCounter++;
        }

        public void AttachContext(HttpListenerContext context)
        {
            HttpContext = context;
            DetectExpectedType();
            SetupDataTransformer();
            SetContentType();
            DetectExpectedType();
        }

        private void SetContentType()
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

        private void SetupDataTransformer()
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
                if(group.Name == "0") // Ignore path match
                {
                    continue;
                }
                PathParameters.Add(group.Name, group.Success ? group.Value : null);
            }
        }
    }
}
