using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HalfShot.MagnetHS.CommonStructures.Caches
{
    public class ProfileCache : IDataCache
    {
        private List<UserProfile> profiles = new List<UserProfile>();

        /// <summary>
        /// A collection of userid+profile_key
        /// </summary>
        private Dictionary<Tuple<string, string>, DateTime> lastUpdatedTime = new Dictionary<Tuple<string, string>, DateTime>();

        public TimeSpan ExpirationTime { get; set; }

        public int Count => profiles.Count;

        public UserProfile Get(UserID id)
        {
            return profiles.Find(pro => pro.UserId == id);
        }

        public ProfileCache(TimeSpan expirationTime) : base()
        {
            ExpirationTime = expirationTime;
        }

        public void UpsertProfileValue(UserID userId, string key, string value)
        {
            UserProfile profile = profiles.Find(pro => pro.UserId == userId);
            if(profile == null)
            {
                profile = new UserProfile() { UserId = userId };
                profiles.Add(profile);
            }
            profile.Profile[key] = value;
            lastUpdatedTime[new Tuple<string, string>(userId.ToString(), key)] = DateTime.Now;
        }

        /// <summary>
        /// Check that none of the keys have expired.
        /// </summary>
        /// <param name="userId">UserID of the profile.</param>
        /// <param name="keys">Keys checked to still be valid.</param>
        /// <returns></returns>
        public bool HasCacheExpired(UserID userId, List<string> keys = null)
        {
            if (keys?.Count > 0)
            {
                return keys.Any(key => {
                    var fullKey = new Tuple<string, string>(userId.ToString(), key);
                    return !lastUpdatedTime.ContainsKey(fullKey) || (DateTime.Now - lastUpdatedTime[fullKey]) > ExpirationTime;
                });
            } else if(profiles.Find(profile => profile.UserId == userId) != null) {
                return !lastUpdatedTime.Where(kv => kv.Key.Item1 == userId.ToString()).All(kv => (DateTime.Now - kv.Value) < ExpirationTime);
            }
            return true;
        }

        public void InvalidateProfileValue(UserID userId, string key)
        {
            lastUpdatedTime[new Tuple<string, string>(userId.ToString(), key)] = DateTime.MinValue;
        }

        public bool Remove(UserProfile item) => profiles.Remove(item);

        public bool Remove(UserID userId) => profiles.RemoveAll((profile) => profile.UserId == userId) > 0;
        
    }
}
