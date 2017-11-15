using System;
using System.Collections.Generic;
using System.Text;
using HalfShot.MagnetHS.CommonStructures;
namespace HalfShot.MagnetHS.DatastoreService
{
    internal class TestMemoryDBPopulator
    {
        public static void PopulateDB()
        {
            Logger.Warn("Populating the DB with TEST data!");
            UserID id = new UserID("@test:localhost");
            using (var datastore = new Datastores.ProfileDatastore(TimeSpan.MinValue))
            {
                datastore.SetProfile(new CommonStructures.Requests.SetProfileRequest()
                {
                    UserId = id,
                    Values = new Dictionary<string, string> {
                        { "displayname", "testuser" },
                        { "avatar_url", "fakeurl" },
                        { "origin", "hardcoded" }
                    }
                });
            }
        }
    }
}
