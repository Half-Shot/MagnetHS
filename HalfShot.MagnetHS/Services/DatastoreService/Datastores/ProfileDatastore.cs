using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures;
using HalfShot.MagnetHS.CommonStructures.Caches;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.DatastoreService.Contexts;
using HalfShot.MagnetHS.DatastoreService.Records;
using HalfShot.MagnetHS.MessageQueue;

namespace HalfShot.MagnetHS.DatastoreService.Datastores
{
    public class ProfileDatastore : IDisposable,  IDatastore
    {
        ProfileCache profileCache;
        public ProfileDatastore(TimeSpan expiryTime)
        {
            profileCache = new ProfileCache(expiryTime);
        }
        
        [DataStoreAttribute(DataStoreOperation.Get)]
        public UserProfile GetProfile(GetProfileRequest request)
        {
            UserProfile profile;
            // Do not use the cache for a full retrieval.
            if (request.Keys.Length > 0 && !profileCache.HasCacheExpired(request.UserId, request.Keys.ToList()))
            {
                profile = profileCache.Get(request.UserId);
            } else {
                profile = getProfileFromRecords(request.UserId, request.Keys);
                foreach (var kvpair in profile.Profile)
                {
                    profileCache.UpsertProfileValue(profile.UserId, kvpair.Key, kvpair.Value);
                }
            }
            if(request.Keys.Length > 0)
            {
                profile.Profile = profile.Profile.Where((kv) => request.Keys.Contains(kv.Key)).ToDictionary(p => p.Key, p => p.Value);
            }
            return profile;
        }

        public void SetProfile(SetProfileRequest request)
        {
            foreach (KeyValuePair<string,string> kv in request.Values)
            {
                setProfileKeyRecord(request.UserId, kv.Key, kv.Value);
            }
        }

        private UserProfile getProfileFromRecords(UserID userId, string[] keys)
        {
            UserProfile profile = new UserProfile() { UserId = userId };
            using (UserStoreContext userContext = new UserStoreContext())
            {
                IQueryable<ProfileRecord> records;
                if(keys.Length > 0) {
                    records = from record in userContext.Profiles
                              where record.UserId == userId.ToString()
                              where keys.Contains(record.Key)
                              select record;
                } else {
                    records = from record in userContext.Profiles
                              where record.UserId == userId.ToString()
                              select record;
                }

                foreach(var record in records)
                {
                    profile.Profile.Add(record.Key, record.Value);
                }
            }
            return profile;
        }

        private void setProfileKeyRecord(UserID userId, string key, string value)
        {
            using (UserStoreContext userContext = new UserStoreContext())
            {
                ProfileRecord record = userContext.Profiles.Find(userId.ToString(), key);
                if (record == null)
                {
                    record = new ProfileRecord() { UserId = userId.ToString(), Key = key, Value = value, CreationDt = DateTime.Now, UpdateDt = DateTime.Now};
                    userContext.Profiles.Add(record);
                } else {
                    record.Value = value;
                    userContext.Profiles.Update(record);
                }
                userContext.SaveChanges();
            }
        }

        public void Dispose()
        {
        }

        public bool CanHandleRequest(MQRequest request)
        {
            return (request is GetProfileRequest) || (request is SetProfileRequest);
        }

        public MQResponse RouteRequest(MQRequest request)
        {
            try
            {
                if (request is GetProfileRequest) {
                    return new ProfileResponse(GetProfile(request as GetProfileRequest));
                } else if (request is SetProfileRequest) {
                    SetProfile(request as SetProfileRequest);
                    return new StatusResponse() { Succeeded = true };
                }
            }
            catch (Exception ex)
            {
                return new StatusResponse() { Succeeded = false, ErrorCode = "HS_DBERROR", Error = ex.Message };
            }
            return null;
        }
    }
}
