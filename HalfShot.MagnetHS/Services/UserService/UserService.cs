using System;
using System.Linq;
using System.Collections.Generic;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.MessageQueue;
namespace HalfShot.MagnetHS.UserService
{
    class UserService
    {
        static IMessageQueue IncomingQueue;
        static IMessageQueue DbQueue;
        private static TimeSpan LoginExpiryTime = new TimeSpan(0, 30, 0);
        static List<string> AcceptedLoginHashesCache;
        static void Main(string[] args)
        {
            IncomingQueue = MQConnector.GetResponder(EMQService.User);
            DbQueue = MQConnector.GetRequester(EMQService.Datastore);
            while (true)
            {
                MQRequest request = IncomingQueue.ListenForRequest();
                MQResponse response = new StatusResponse() { ErrorCode = "HS_NOHANDLER", Error = "There is no handler for this request type.", Succeeded = false };
                try
                {
                    switch (request.GetType().Name)
                    {
                        case "GetProfileRequest":
                            response = HandleGetProfileRequest(request as GetProfileRequest);
                            break;
                        case "SetProfileRequest":
                            response = HandleSetProfileRequest(request as SetProfileRequest);
                            break;
                        case "LoginRequest":
                            response = HandleLoginRequest(request as LoginRequest);
                            break;
                        default:
                            break;
                    }
                }
                catch (TimeoutException)
                {
                    response = StatusResponse.StandardTimeoutResponse("Datastore");
                }

                IncomingQueue.Respond(response);
            }
        }

        static StatusResponse HandleSetProfileRequest(SetProfileRequest request)
        {
            //TODO: Validate keys
            DbQueue.Request(request);
            return (StatusResponse)DbQueue.ListenForResponse();
        }

        static MQResponse HandleGetProfileRequest(GetProfileRequest request)
        {
            DbQueue.Request(request);
            var response = DbQueue.ListenForResponse();
            return response;
        }

        static MQResponse HandleLoginRequest(LoginRequest request)
        {
            if(request.Type == CommonStructures.Enums.ELoginType.Password)
            {
                var passwordCheck = new CheckPasswordRequest() { Password = request.Token, UserId = request.UserId };
                DbQueue.Request(passwordCheck);
                var password_check_status = DbQueue.ListenForResponse() as StatusResponse;
                if (password_check_status.Succeeded)
                {
                    string deviceId;
                    if(request.DeviceId == null)
                    {
                        // Create a device too.
                        deviceId = "fakedeviceid";
                    } else {
                        deviceId = request.DeviceId;
                    }

                    DbQueue.Request(new CreateAccessTokenRequest()
                    {
                        DeviceId = deviceId,
                        UserId = request.UserId,
                        ExpiryDateTime = DateTime.Now + LoginExpiryTime,
                    });
                    var accesstoken_response = DbQueue.ListenForResponse() as AccessTokenResponse;
                    return new LoginResponse()
                    {
                        UserId = request.UserId,
                        AccessToken = accesstoken_response.AccessTokens.First(),
                        DeviceId = accesstoken_response.DeviceIds.First()
                    };
                }
                else
                {
                    return password_check_status;
                }
            }
            else
            {
                return new StatusResponse() { ErrorCode = "HS_NOHANDLER", Error = "Login type not supported.", Succeeded = false, Stubbed = true };
            }
        }
    }
}
