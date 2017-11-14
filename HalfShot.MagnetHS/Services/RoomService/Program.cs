using System;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
namespace HalfShot.MagnetHS.RoomService
{
    class Program
    {
        static IMessageQueue IncomingQueue;
        static IMessageQueue DbQueue;
        static void Main(string[] args)
        {
            IncomingQueue = MQConnector.GetResponder(EMQService.Room);
            DbQueue = MQConnector.GetRequester(EMQService.Datastore);
            while (true)
            {
                MQRequest request = IncomingQueue.ListenForRequest();
                MQResponse response = new StatusResponse() { ErrorCode = "HS_NOHANDLER", Error = "There is no handler for this request type.", Succeeded = false };
                switch (request.GetType().Name)
                {
                    case "GetRoomEvents":
                        response = HandleGetRoomEvents(request as GetRoomEvents);
                        break;
                    default:
                        break;
                }
                IncomingQueue.Respond(response);
            }
        }

        static MQResponse HandleGetRoomEvents(GetRoomEvents request)
        {
            return new StatusResponse() { Stubbed = true, Succeeded = false };
        }
    }
}
