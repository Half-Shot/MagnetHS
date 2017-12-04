using HalfShot.MagnetHS.MessageQueue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HalfShot.MagnetHS.ClientServerAPIService.Exceptions;
using HalfShot.MagnetHS.ClientServerAPIService.Requests;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Enums;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.CommonStructures.Responses.Room;
using HalfShot.MagnetHS.CommonStructures.Room;
using HalfShot.MagnetHS.CommonStructures.ServiceClient;
using RoomResponse = HalfShot.MagnetHS.ClientServerAPIService.Responses.RoomResponse;

namespace HalfShot.MagnetHS.ClientServerAPIService.Controllers
{
    [RestPath("r0/createRoom")]
    class CreateRoomController : RestController
    {
        public CreateRoomController()
        {
            
        }

        [RestEndPoint("POST", null, true)]
        public void CreateRoom(RestContext context)
        {
            var request = ValidateRequest(context);
            using (var serviceClient = new RoomServiceClient())
            {
                RoomID roomId = null;
                var opts = new RoomCreationOpts()
                {
                    Name = request.name,
                    Topic = request.topic
                };
                try
                {
                    roomId = serviceClient.CreateRoom(context.AuthenticatedUser, opts);
                }
                catch (ServiceFailureException e)
                {
                    throw new RestError(e.Response.Error, e.Response.ErrorCode);
                }
                catch (TimeoutException ex)
                {
                    throw new RestError($"Timed out while waiting for a response from the RoomService.", StatusResponse.ServiceTimeout, ex);
                }
                catch (Exception e)
                {
                    throw e;
                }
                
                using (var stream = context.DataTransformer.ToStream(new RoomResponse()
                {
                    room_id = roomId.ToString()
                }))
                {
                    stream.CopyTo(context.HttpContext.Response.OutputStream);
                }
                context.HttpContext.Response.Close();
            }
        }
        
        private CreateRoomRequest ValidateRequest(RestContext context)
        {
            CreateRoomRequest createRequest = null;
            try
            {
                using (var stream = new StreamReader(context.HttpContext.Request.InputStream))
                {
                    createRequest = context.DataTransformer.FromStream<CreateRoomRequest>(stream);
                }
                if (createRequest == null)
                {
                    throw new ArgumentNullException("Keys should not be null");
                }
            }
            catch (Exception ex) //TODO: Detect an actual parse error.
            {
                throw new RestError($"Request did not contain valid json.", "M_NOT_JSON", ex);
            }
            return createRequest;
        }
    }
}
