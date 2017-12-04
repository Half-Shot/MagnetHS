using System;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Requests.Room;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Responses.Room;

namespace HalfShot.MagnetHS.RoomService
{
    class Program
    {
        //public static IMessageQueue FederationRequest;
        private static RoomHandler handler;
        static IMessageQueue IncomingQueue;
        static void Main(string[] args)
        {
            //FederationRequest =  MQConnector.GetResponder(EMQService.FederationRequest);
            IncomingQueue = MQConnector.GetResponder(EMQService.Room);
            DbRoomEventStore store = new DbRoomEventStore();
            handler = new RoomHandler();
            while (true)
            {
                MQRequest request = IncomingQueue.ListenForRequest();
                MQResponse response = new StatusResponse() { ErrorCode = "HS_NOHANDLER", Error = "There is no handler for this request type.", Succeeded = false };
                switch (request.GetType().Name)
                {
                    case "GetRoomEvents":
                        response = HandleGetRoomEvents(request as GetRoomEvents);
                        break;
                    case "CreateRoomRequest":
                        response = HandleCreateRoomRequest(request as CreateRoomRequest);
                        break;
                }
                IncomingQueue.Respond(response);
            }
        }

        static MQResponse HandleGetRoomEvents(GetRoomEvents request)
        {
            return new StatusResponse() { Stubbed = true, Succeeded = false };
        }

        static MQResponse HandleCreateRoomRequest(CreateRoomRequest request)
        {
            try
            {
                var roomId = handler.CreateRoom(request.Sender, request.Opts);
                return new RoomResponse() {RoomId = roomId};
            }
            catch (Exception e)
            {
                return new StatusResponse() {Succeeded = false, Error = e.Message};
            }
        }
    }
}
