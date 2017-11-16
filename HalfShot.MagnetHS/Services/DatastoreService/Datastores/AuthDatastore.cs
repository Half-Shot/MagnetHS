using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security;
using HalfShot.MagnetHS.MessageQueue;
using HalfShot.MagnetHS.CommonStructures.Requests;
using HalfShot.MagnetHS.CommonStructures.Responses;
using HalfShot.MagnetHS.DatastoreService.Contexts;
using HalfShot.MagnetHS.DatastoreService.Records;
using System.Security.Cryptography;

namespace HalfShot.MagnetHS.DatastoreService.Datastores
{
    class PasswordDatastore : IDisposable, IDatastore
    {

        public StatusResponse SetPassword(SetPasswordRequest request)
        {
            using (UserStoreContext userContext = new UserStoreContext())
            {
                byte[] salt = PasswordHasher.GenerateSalt();
                byte[] hash = PasswordHasher.HashPassword(request.Password, salt);
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
                byte[] hash = PasswordHasher.HashPassword(request.Password, userRecord.Salt);
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

        public bool CanHandleRequest(MQRequest request)
        {
            return (request is CheckPasswordRequest) || (request is SetPasswordRequest);
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
