using System;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.MessageQueue;
namespace HalfShot.MagnetHS.UserService
{
    class UserService
    {
        static IMessageQueue IncomingQueue;
        static IMessageQueue DbQueue;
        static void Main(string[] args)
        {
            IncomingQueue = MQConnector.GetResponder(EMQService.User);
            DbQueue = MQConnector.GetRequester(EMQService.Datastore);
            while (true)
            {
                MQRequest request = IncomingQueue.ListenForRequest();
                MQResponse response = new StatusResponse() { ErrorCode = "HS_NOHANDLER", Error = "There is no handler for this request type.", Succeeded = false }; 
                switch (request.GetType().Name)
                {
                    case "GetProfileRequest":
                        response = HandleGetProfileRequest(request as GetProfileRequest);
                        break;
                    case "SetProfileRequest":
                        response = HandleSetProfileRequest(request as SetProfileRequest);
                        break;
                    default:
                        break;
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
    }
}
