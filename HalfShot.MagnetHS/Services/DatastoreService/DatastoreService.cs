using System;
using System.Reflection;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.DatastoreService.Datastores;
using HalfShot.MagnetHS.MessageQueue;
using System.Collections.Generic;

namespace HalfShot.MagnetHS.DatastoreService
{
    public class DatastoreService
    {
        public const string DBNAME = "MagnetHSDB";
        static IMessageQueue IncomingQueue;
        static List<IDatastore> datastores;
        static void Main(string[] args)
        {
            Logger.StartLogger();
            Logger.Info("Started DatastoreService");
            TestMemoryDBPopulator.PopulateDB();
            datastores = new List<IDatastore>();
            datastores.Add(new ProfileDatastore(TimeSpan.FromSeconds(30)));
            datastores.Add(new EventDatastore());
            datastores.Add(new AuthDatastore());
            IncomingQueue = MQConnector.GetResponder(EMQService.Datastore);
            MQRequest request;
            while (true)
            {
                request = IncomingQueue.ListenForRequest();
                Logger.Debug($"Got request {request.GetType().Name}");
                foreach (IDatastore datastore in datastores)
                {
                    if (datastore.CanHandleRequest(request))
                    {
                        Logger.Debug($"Request is being handled by {datastore.GetType().Name}");
                        var response = datastore.RouteRequest(request);
                        IncomingQueue.Respond(response);
                        Logger.Debug($"Request handled");
                        break;
                    }
                }
            }
        }
    }
}
