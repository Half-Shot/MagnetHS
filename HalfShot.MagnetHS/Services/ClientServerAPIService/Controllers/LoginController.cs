using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HalfShot.MagnetHS.ClientServerAPIService.Exceptions;
using HalfShot.MagnetHS.ClientServerAPIService.Requests;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Responses;

namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("r0/login")]
    class LoginController : RestController
    {
        IMessageQueue userMQ;

        private static List<string> SupportedLoginTypes = new List<string> { "m.login.password", "m.login.token" };

        public LoginController()
        {
            userMQ = MQConnector.GetRequester(EMQService.User);
        }

        [RestEndPoint("POST")]
        public void Login(RestContext context)
        {
            LoginRequest loginRequest = ValidateRequest(context);
            CommonStructures.Requests.LoginRequest mqLoginRequest = null;
            // Get the access token
            if (loginRequest.Type == ELoginType.Password)
            {
                mqLoginRequest = new CommonStructures.Requests.LoginRequest()
                {
                    Type = ELoginType.Password,
                    UserId = new UserID(loginRequest.user),
                    Token = loginRequest.password,
                    DeviceId = loginRequest.device_id
                };
            } else if (loginRequest.Type == ELoginType.Token) {
                mqLoginRequest = new CommonStructures.Requests.LoginRequest()
                {
                    Type = ELoginType.Password,
                    UserId = new UserID(loginRequest.user),
                    Token = loginRequest.password
                };
            }

            MQResponse mqLoginResponse;
            object response = null;
            userMQ.Request(mqLoginRequest);
            try
            {
                mqLoginResponse = userMQ.ListenForResponse();
                if(mqLoginResponse is LoginResponse)
                {
                    var loginResponse = mqLoginResponse as LoginResponse;
                    response = new Responses.LoginResponse()
                    {
                        user_id = loginResponse.UserId.ToString(),
                        access_token = loginResponse.AccessToken,
                        home_server = "localhost",
                        device_id = loginResponse.DeviceId,
                    };
                    context.HttpContext.Response.StatusCode = 200;
                }
                else if (mqLoginResponse is StatusResponse)
                {
                    var statusCode = mqLoginResponse as StatusResponse;
                    context.HttpContext.Response.StatusCode = statusCode.ErrorCode == "M_FORBIDDEN" ? 401 : 500;
                    response = new Responses.ErrorResponse()
                    {
                        error = statusCode.Error,
                        errcode = statusCode.ErrorCode,
                    };
                }

                using (var stream = context.DataTransformer.ToStream(response))
                {
                    stream.CopyTo(context.HttpContext.Response.OutputStream);
                }
                context.HttpContext.Response.Close();
            }
            catch (TimeoutException ex)
            {
                Logger.Warn("Timeout occured trying to talk to the UserService.", context.RequestId);
                throw new RestError($"Timed out while waiting for a response from the UserService.", StatusResponse.ServiceTimeout, ex);
            }
        }

        private LoginRequest ValidateRequest(RestContext context)
        {
            LoginRequest loginRequest = null;
            try
            {
                using (var stream = new StreamReader(context.HttpContext.Request.InputStream))
                {
                    loginRequest = context.DataTransformer.FromStream<LoginRequest>(stream);
                }
                if (loginRequest == null)
                {
                    throw new ArgumentNullException("Keys should not be null");
                }
            }
            catch (Exception ex) //TODO: Detect an actual parse error.
            {
                throw new RestError($"Request did not contain valid json.", "M_NOT_JSON", ex);
            }

            if (String.IsNullOrEmpty(loginRequest.type))
            {
                throw new RestError($"Missing 'type'.", "M_BAD_JSON");
            }

            if (loginRequest.Type == ELoginType.Unknown)
            {
                throw new RestError($"'type' was not one of {String.Join(", ", SupportedLoginTypes)}.", "M_UNKNOWN");
            }

            if (loginRequest.Type == ELoginType.Password)
            {
                if (String.IsNullOrEmpty(loginRequest.password))
                {
                    throw new RestError($"Missing 'password'.", "M_BAD_JSON");
                }
                if (String.IsNullOrEmpty(loginRequest.user))
                {
                    throw new RestError($"Missing 'password'.", "M_BAD_JSON");
                }
                try
                {
                    new UserID(loginRequest.user);
                }
                catch (FormatException ex)
                {
                    throw new RestBadParameters("userid", ex);
                }
            }
            else if (loginRequest.Type == ELoginType.Token && String.IsNullOrEmpty(loginRequest.token))
            {
                throw new RestError($"Missing 'token'.", "M_BAD_JSON");
            }

            return loginRequest;
        }
    }
}
