using System;
using System.Runtime.InteropServices.WindowsRuntime;
using HalfShot.MagnetHS.CommonStructures.Requests.Room;
using HalfShot.MagnetHS.CommonStructures.Requests.User;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Responses.Room;
using HalfShot.MagnetHS.CommonStructures.Responses.User;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.MessageQueue;

namespace HalfShot.MagnetHS.CommonStructures.ServiceClient
{
    public class UserServiceClient : IServiceClient, IDisposable
    {
        private readonly IMessageQueue _messageQueue;

        public UserServiceClient()
        {
            _messageQueue = MQConnector.GetRequester(EMQService.User);
        }

        public UserID GetUserFromToken(string accessToken)
        {
            _messageQueue.Request(new GetAccessTokenUser()
            {
               AccessToken = accessToken
            });
            var response  = _messageQueue.ListenForResponse();
            if (response is UserResponse)
            {
                return (response as UserResponse).UserId;
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