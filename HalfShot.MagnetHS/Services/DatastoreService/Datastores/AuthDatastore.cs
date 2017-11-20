using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.DatastoreService.Contexts;
using HalfShot.MagnetHS.DatastoreService.Records;
using HalfShot.MagnetHS.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HalfShot.MagnetHS.DatastoreService.Datastores
{
    class AuthDatastore : IDisposable, IDatastore
    {

        public StatusResponse SetPassword(SetPasswordRequest request)
        {
            using (UserStoreContext userContext = new UserStoreContext())
            {
                byte[] salt = HashingProvider.GenerateSalt();
                byte[] hash = HashingProvider.HashPassword(request.Password, salt);
                PasswordRecord record = userContext.Passwords.Find(request.UserId.ToString());
                if (record == null)
                {
                    record = new PasswordRecord() { UserId = request.UserId.ToString(), Hash = hash, Salt = salt, CreationDt = DateTime.Now, UpdateDt = DateTime.Now };
                    userContext.Passwords.Add(record);
                }
                else
                {
                    record.Hash = hash;
                    record.Salt = salt;
                    record.UpdateDt = DateTime.Now;
                    userContext.Passwords.Update(record);
                }
                userContext.SaveChanges();
            }
            return new StatusResponse() { Succeeded = true };
        }

        public StatusResponse CheckPassword(CheckPasswordRequest request)
        {
            using (UserStoreContext userContext = new UserStoreContext())
            {
                var userRecords = from record in userContext.Passwords
                                  where record.UserId == request.UserId.ToString()
                                  select record;

                if(userRecords.Count() == 0)
                {
                    return new StatusResponse() { Succeeded = false, ErrorCode = "M_NOT_FOUND", Error = "User was not found" };
                }
                var userRecord = userRecords.First();
                Logger.Debug($"Checking password '{request.Password}' for {request.UserId}");
                byte[] hash = HashingProvider.HashPassword(request.Password, userRecord.Salt);
                if(hash.SequenceEqual(userRecord.Hash))
                {
                    return new StatusResponse() { Succeeded = true };
                }
                else
                {
                    return new StatusResponse() { Succeeded = false, ErrorCode = "M_FORBIDDEN", Error = "Hash did not match" };
                }
            }
        }

        public AccessTokenResponse CreateAccessToken(CreateAccessTokenRequest request)
        {
            using (UserStoreContext userContext = new UserStoreContext())
            {
                var record = new AccessTokenRecord()
                {
                    AccessToken = HashingProvider.GenerateAccessToken(),
                    UserId = request.UserId.ToString(),
                    DeviceId = request.DeviceId,
                    CreationDt = DateTime.Now,
                    ExpiryDt = request.ExpiryDateTime
                };
                userContext.AddAsync(record).ContinueWith((t) =>
                {
                    userContext.SaveChangesAsync();
                });
                return new AccessTokenResponse()
                {
                    UserId = request.UserId,
                    AccessTokenDevices = new Dictionary<string, string>() { { record.AccessToken, record.DeviceId } }
                };
            }
        }

        public AccessTokenResponse GetAccessTokens(GetAccessTokensRequest request)
        {
            using (UserStoreContext userContext = new UserStoreContext())
            {
                var records = userContext.AccessTokens.Where(
                    (rec) => rec.UserId == request.UserId.ToString()
                    && (request.DeviceId == null || request.DeviceId == rec.DeviceId)
                    && rec.ExpiryDt > DateTime.Now
                );
                Dictionary<string, string> accessTokens = new Dictionary<string, string>();
                foreach (var record in records)
                {
                    accessTokens.Add(record.AccessToken, record.DeviceId);
                }
                return new AccessTokenResponse()
                {
                    UserId = request.UserId,
                    AccessTokenDevices = accessTokens
                };
            }
        }

        public bool CanHandleRequest(MQRequest request)
        {
            return (request is CheckPasswordRequest) || (request is SetPasswordRequest) || (request is GetAccessTokensRequest) || (request is CreateAccessTokenRequest);
        }

        public MQResponse RouteRequest(MQRequest request)
        {
            try
            {
                if (request is CheckPasswordRequest)
                {
                    return CheckPassword(request as CheckPasswordRequest);
                }
                else if(request is SetPasswordRequest)
                {
                    return SetPassword(request as SetPasswordRequest);
                }
                else if(request is GetAccessTokensRequest)
                {
                    return GetAccessTokens(request as GetAccessTokensRequest);
                }
                else if (request is CreateAccessTokenRequest)
                {
                    return CreateAccessToken(request as CreateAccessTokenRequest);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Encountered an exeption while doing a DB operation. {ex.Message}");
                return new StatusResponse() { Succeeded = false, ErrorCode = "HS_DBERROR", Error = ex.Message };
            }
            return null;
        }

        public void Dispose()
        {

        }
    }
}
