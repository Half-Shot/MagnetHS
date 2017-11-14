using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.ClientServerAPIService.Exceptions;
namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("profile")]
    class ProfileController : RestController
    {
        IMessageQueue userMQ;

        public ProfileController()
        {
            userMQ = MQConnector.GetRequester(EMQService.User);
        }

        [RestEndPoint("GET", "(?<userid>.+)/(?<key>.+)", true)]
        public void GetProfileKeyed(RestContext context)
        {
            getProfileByKeys(context, context.PathParameters["key"]);
        }

        [RestEndPoint("PUT", "(?<userid>.+)/(?<key>.+)", true)]
        public void PutProfileKeyed(RestContext context)
        {
            var request = new SetProfileRequest();
            try
            {
                request.UserId = new CommonStructures.UserID(context.PathParameters["userid"]);
            }
            catch (FormatException ex)
            {
                throw new RestBadParameters("userid", ex);
            }
            var key = context.PathParameters["key"];
            Dictionary<string, string> keys;
            try
            {
                keys = context.DataTransformer.FromStream<Dictionary<string, string>>(context.HttpContext.Request.InputStream);
                if(keys == null)
                {
                    throw new ArgumentNullException("Keys should not be null");
                }
            }
            catch (Exception) //TODO: Detect an actual parse error.
            {
                throw new RestError($"Request did not contain valid json.", "M_NOT_JSON");
                throw;
            }
            
            if (!keys.ContainsKey(key)){
                throw new RestError($"Missing {key} key.", "M_BAD_JSON");
            }

            
            request.Values.Add(key, keys[key]);
            userMQ.Request(request);
            StatusResponse response;
            try
            {
                response = userMQ.ListenForResponse() as StatusResponse;
            }
            catch (TimeoutException ex)
            {
                throw new RestError($"Timed out while waiting for a response from the UserService.", "HalfShot_MagnetHS_ServiceTimeout", ex);
            }
            
            if(response.Succeeded) {
                context.HttpContext.Response.StatusCode = 200;
            } else {
                context.HttpContext.Response.StatusCode = 500;
                using (var stream = context.DataTransformer.ToStream(response as StatusResponse))
                {
                    stream.CopyTo(context.HttpContext.Response.OutputStream);
                }
            }
            context.HttpContext.Response.Close();
        }

        [RestEndPoint("GET", "(?<userid>.+)", true)]
        public void GetProfile(RestContext context)
        {
            getProfileByKeys(context);
        }

        private void getProfileByKeys(RestContext context,params string[] keys)
        {
            GetProfileRequest profileRequest = new GetProfileRequest();

            try
            {
                profileRequest.UserId = new CommonStructures.UserID(context.PathParameters["userid"]);
            }
            catch (FormatException ex)
            {
                throw new RestBadParameters("userid", ex);
            }
            
            profileRequest.Keys = keys;
            userMQ.Request(profileRequest);
            MQResponse user_response;

            try
            {
                user_response = userMQ.ListenForResponse();
            }
            catch (TimeoutException ex)
            {
                throw new RestError($"Timed out while waiting for a response from the UserService.", "HalfShot_MagnetHS_ServiceTimeout", ex);
            }
            
            object cl_response = null;
            if (user_response is ProfileResponse)
            {
                var profile = (user_response as ProfileResponse).Profile;
                if(profile.Profile.Count == 0)
                {
                    cl_response = new Responses.ErrorResponse()
                    {
                        errcode = "M_NOT_FOUND",
                        error = keys.Length > 0 ? "No profile found with the given key" : "No profile found"
                    };
                    context.HttpContext.Response.StatusCode = 404;
                } else
                {
                    cl_response = (user_response as ProfileResponse).Profile.Profile;
                }
            } else if(user_response is StatusResponse) {
                //TODO: Report this error.
                cl_response = new Responses.ErrorResponse()
                {
                    errcode = "M_UNKNOWN",
                    error = "An error occured while trying to retrieve the profile.",
                    debug = (user_response as StatusResponse).Error
                };
            }
            using (var stream = context.DataTransformer.ToStream(cl_response))
            {
                stream.CopyTo(context.HttpContext.Response.OutputStream);
            }
            context.HttpContext.Response.Close();
        }
    }
}
