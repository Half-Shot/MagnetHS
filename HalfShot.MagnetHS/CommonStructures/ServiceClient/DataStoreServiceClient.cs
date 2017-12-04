using System;
using System.Collections.Generic;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.MessageQueue;

namespace HalfShot.MagnetHS.CommonStructures.ServiceClient
{
    public class DataStoreServiceClient : IServiceClient, IDisposable
    {  
        private readonly IMessageQueue _messageQueue;

        public DataStoreServiceClient()
        {
            _messageQueue = MQConnector.GetRequester(EMQService.Datastore);
        }

        public UserProfile GetProfile(UserID user, List<string> keys)
        {
            
        }
        
        public void SetProfile(UserID user, List<string> keys)
        {
            
        }
        
        public bool CheckPassword(UserID user, string password)
        {
            var passwordCheck = new CheckPasswordRequest() { Password = password, UserId = user };
            _messageQueue.Request(passwordCheck);
            var password_check_status = _messageQueue.ListenForResponse() as StatusResponse;
            if (password_check_status.Error != null)
            {
                throw new ServiceFailureException(password_check_status);
            }
            return password_check_status.Succeeded;
        }
        
        public string CreateAccountToken(UserID userId, string deviceId, TimeSpan expieryTime)
        {
            _messageQueue.Request(new CreateAccessTokenRequest()
            {
                DeviceId = deviceId,
                UserId = userId,
                ExpiryDateTime = DateTime.Now + expieryTime,
            });
            if (_messageQueue.ListenForResponse() is AccessTokenResponse)
            {
                ref 
            };
        }

        public string CreateDevice(UserID user)
        {
            
        }
        
        public void Dispose()
        {
            _messageQueue.Dispose();
        }
    }
}