using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Reflection;
using System.Linq;
using HalfShot.MagnetHS.ClientServerAPIService.Exceptions;
using HalfShot.MagnetHS.ClientServerAPIService.Responses;
using HalfShot.MagnetHS.CommonStructures.ServiceClient;

namespace HalfShot.MagnetHS.ClientServerAPIService
{
    abstract class RestController
    {
        protected string RootPath = "";
        private List<string> rootSegments;
        public RestController() {
            RootPath = GetType().GetCustomAttribute<RestPath>().Path;
            rootSegments = new List<string>(RootPath.Split('/')).Where(
                segment => !String.IsNullOrEmpty(segment) || segment == "/").ToList();
        }

        public bool HandleRequest(HttpListenerContext context, string relativePath)
        {
            if (!IsURIHandled(relativePath)) //Is the given path relative to this controller?
            {
                return false;
            }

            RestContext ctx = GetRestContext(context);
            if(ctx == null)
            {
                return false;
            }

            relativePath = relativePath.Trim('/').Substring(RootPath.Length).Trim('/');

            
            bool MethodNotSupported = false;
            foreach (MethodInfo method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
            {
                var endpoint = method.GetCustomAttribute<RestEndPointAttribute>();
                if (endpoint != null && endpoint.URLMatch(relativePath))
                {
                    if (endpoint.Method.ToUpper() == context.Request.HttpMethod.ToUpper())
                    {
                        ctx.ProcessPathParameters(endpoint.Endpoint, relativePath);
                        try
                        {
                            if(endpoint.AuthRequired)
                            {
                                RestError badLoginResponse;
                                if (!AuthenticateUser(ctx, out badLoginResponse))
                                {
                                    throw badLoginResponse;
                                }
                            }
                            InvokeRestMethod(method, ctx);
                        }
                        catch (RestError ex)
                        {
                            Logger.Debug($"RestError encountered processing request: {ex.Message}", ctx.RequestId);
                            Logger.Debug($"Inner exception: {ex.InnerException?.Message}", ctx.RequestId);
                            context.Response.StatusCode = ex.StatusCode;
                            using (var stream = ctx.DataTransformer.ToStream(new ErrorResponse()
                            {
                                errcode = ex.ErrorCode,
                                error = ex.Message
                            }))
                            {
                                stream.CopyTo(ctx.HttpContext.Response.OutputStream);
                            }
                            ctx.HttpContext.Response.OutputStream.Close();
                        }
                        catch(Exception ex)
                        {
                            Logger.Error($"A exception fell through while processing a request: {ex.ToString()} {ex.Message}", ctx.RequestId);
                            context.Response.StatusCode = 500;
                            using (var stream = ctx.DataTransformer.ToStream(new ErrorResponse()
                            {
                                errcode = "M_UNKNOWN",
                                error = "A fatal internal error occured while handling the request"
                            }))
                            {
                                stream.CopyTo(ctx.HttpContext.Response.OutputStream);
                            }
                            ctx.HttpContext.Response.OutputStream.Close();
                        }
                        return true;
                    }
                    else
                    {
                        MethodNotSupported = true;
                    }
                }
            }
            GetError(
                context,
                MethodNotSupported ? 405 : 404,
                MethodNotSupported ? "Method not supported" : "Endpoint not found"
                );
            return true;

        }

        private bool IsURIHandled(string relativePath)
        {
            var segments = new List<string>(relativePath.Split('/')).Where(
                segment => !String.IsNullOrEmpty(segment) || segment == "/").ToList();
            for (int i = 0; i < rootSegments.Count; i++)
            {
                if(segments[i].ToLower() != rootSegments[i].ToLower())
                {
                    return false;
                }
            }
            return true;
        }

        private RestContext GetRestContext(HttpListenerContext context)
        {
            try
            {
                var ctx = new RestContext();
                ctx.AttachContext(context);
                return ctx;
            }
            catch (RestContentTypeNotSupported ex)
            {
                GetError(context, 415, ex.Message);
                return null;
            }
            catch (Exception)
            {
                GetError(context, 500, "An error occured while handling the request.");
                throw;
            }
        }

        private bool AuthenticateUser(RestContext context, out RestError err)
        {
            err = null;
            string access_token = context.HttpContext.Request.QueryString.Get("access_token");
            // Fall back to header.
            if(access_token == null)
            {
                access_token = context.HttpContext.Request.Headers.Get("Authorization");
                if (access_token == null)
                {
                    err = new RestError("Missing access token.", "M_MISSING_TOKEN") { StatusCode = 401 };
                    return false;
                }
                if(!access_token.StartsWith("Bearer "))
                {
                    err = new RestError("Authorization in the wrong format.", "M_UNKNOWN_TOKEN") { StatusCode = 401 };
                    return false;
                }
                access_token = access_token.Substring("Bearer ".Length);
            }
            using (var userService = new UserServiceClient())
            {
                var user = userService.GetUserFromToken(access_token);
                if (user == null) return false;
                context.AuthenticatedUser = user;
                return true;
            } 
        }

        //protected bool TryReadPayload(RestContext context, out object obj)
        //{
        //    // Get type by content-type.
        //    context.SuggestedContentType
        //}

        protected void WritePayload(RestContext context)
        {

        }

        public void RegisterEndpoint(HttpListener listener, string root)
        {
            listener.Prefixes.Add(root + "/" + RootPath + "/");
        }

        protected virtual void InvokeRestMethod(MethodInfo method, RestContext context)
        {
            try
            {
                method.Invoke(this, new object[] { context });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            
        }

        protected virtual void GetError(HttpListenerContext context, int status, string error)
        {
            context.Response.StatusCode = status;
            context.Response.StatusDescription = error;
            byte[] data = Encoding.UTF8.GetBytes(error);
            context.Response.OutputStream.Write(data, 0, data.Length);
            context.Response.Close();
        }
    }
}
