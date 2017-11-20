using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests.Datastore;
using HalfShot.MagnetHS.CommonStructures.Responses;

namespace HalfShot.MagnetHS.DatastoreService.Datastores
{
    class EventDatastore : IDatastore
    {
        public bool CanHandleRequest(MQRequest request)
        {
            return (request is GetEventsRequest);
        }

        public MQResponse RouteRequest(MQRequest request)
        {
            return new StatusResponse()
            {
                Error = "RouteRequest is stubbed",
                ErrorCode = "HS_STUBBED",
                Succeeded = false,
                Stubbed = true                
            };
        }
    }
}
