using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.MessageQueue;
namespace HalfShot.MagnetHS.DatastoreService
{
    interface IDatastore
    {
        bool CanHandleRequest(MQRequest request);
        MQResponse RouteRequest(MQRequest request);
    }
}
