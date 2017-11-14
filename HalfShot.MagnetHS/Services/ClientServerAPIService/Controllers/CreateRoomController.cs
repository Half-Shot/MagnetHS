using HalfShot.MagnetHS.MessageQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("createRoom")]
    class CreateRoomController : RestController
    {
        IMessageQueue roomMQ;

        public CreateRoomController()
        {
            roomMQ = MQConnector.GetRequester(EMQService.Room);
        }

        [RestEndPoint("POST")]
        public void CreateRoom(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }
    }
}
