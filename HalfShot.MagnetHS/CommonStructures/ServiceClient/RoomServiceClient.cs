using System;
using System.Runtime.InteropServices.WindowsRuntime;
using HalfShot.MagnetHS.CommonStructures.Requests.Room;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Responses.Room;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.MessageQueue;

namespace HalfShot.MagnetHS.CommonStructures.ServiceClient
{
    public class RoomServiceClient : IServiceClient, IDisposable
    {
        private readonly IMessageQueue _messageQueue;

        public RoomServiceClient()
        {
            _messageQueue = MQConnector.GetRequester(EMQService.Room);
        }

        public RoomID CreateRoom(UserID sender, RoomCreationOpts opts)
        {
            _messageQueue.Request(new CreateRoomRequest()
            {
                Sender = sender,
                Opts = opts
            });
            var response  = _messageQueue.ListenForResponse();
            if (response is RoomResponse)
            {
                return (response as RoomResponse).RoomId;
            }
            else if (response is StatusResponse)
            {
               throw new ServiceFailureException(response as StatusResponse);
            }
            throw new Exception("Unexpected response from RoomService");
        }
        
        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}