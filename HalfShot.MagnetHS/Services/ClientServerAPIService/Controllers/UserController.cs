using HalfShot.MagnetHS.MessageQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("r0/user")]
    class UserController : RestController
    {
        IMessageQueue userMQ;

        public UserController()
        {
            userMQ = MQConnector.GetRequester(EMQService.User);
        }

        [RestEndPoint("PUT", "(?<userid>.+)/account_data/(?<type>.+)", true)]
        public void PutAccountData(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("PUT", "(?<userid>.+)/rooms/(?<roomid>.+)/account_data/(?<type>.+)", true)]
        public void PutAccountDataRoom(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("GET", "(?<userid>.+)/rooms/(?<roomId>.+)/tags", true)]
        public void GetTags(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("PUT", "(?<userid>.+)/rooms/(?<roomId>.+)/tags/(?<tag>.+)", true)]
        public void PutTag(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("DELETE", "(?<userid>.+)/rooms/(?<roomId>.+)/tags/(?<tag>.+)", true)]
        public void DeleteTag(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }
    }
}
