using HalfShot.MagnetHS.MessageQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("rooms")]
    class RoomController : RestController
    {
        IMessageQueue roomMQ;

        public RoomController()
        {
            roomMQ = MQConnector.GetRequester(EMQService.Room);
        }

        [RestEndPoint("GET", "(?<roomId>.+)/state", true)]
        public void GetRoomState(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("GET", "(?<roomId>.+)/state/(?<eventType>.+)")]
        public void GetRoomStateOfType(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("GET", "(?<roomId>.+)/state/(?<eventType>.+)/?<stateKey>.+)")]
        public void GetRoomStateOfTypeAndKey(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("GET", "(?<roomId>.+)/members")]
        public void GetRoomMembers(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("GET", "(?<roomId>.+)/messages")]
        public void GetRoomMessages(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("PUT", "(?<roomId>.+)/state/(?<eventType>.+)")]
        public void PutRoomState(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("PUT", "(?<roomId>.+)/state/(?<eventType>.+)/?<stateKey>.+)")]
        public void PutRoomStateKeyed(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("PUT", "(?<roomId>.+)/send/(?<eventType>.+)/?<txnId>.+)")]
        public void PutRoomEvent(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        [RestEndPoint("PUT", "(?<roomId>.+)/redact/(?<eventType>.+)/?<txnId>.+)")]
        public void RedactRoomEvent(RestContext context)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        /*[RestEndPoint("GET", "(?<roomId>.+)/initialSync")]
        public void GetRoomInitialSync(RestContext context)
        {

        }*/
    }
}
