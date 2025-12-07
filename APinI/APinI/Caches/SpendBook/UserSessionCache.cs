using APinI.Models.SpendBook;
using Microsoft.Extensions.Caching.Memory;

namespace APinI.Caches.SpendBook
{
    public class UserSessionCache : IUserSessionCache
    {
        private IMemoryCache _memoryCache;
        public UserSessionCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public UserInfo GetUserInfoByUsername(string username)
        {
            var key = GetKey(username);
            if (_memoryCache.TryGetValue(key, out UserInfo userInfo))
            {
                return userInfo;
            }
            throw new KeyNotFoundException($"User session for {username} not found.");
        }

        public void SetUserSessionCache(UserInfo userInfo)
        {
            var key = GetKey(userInfo.Username.ToLower());
            _memoryCache.Set(key, userInfo, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1)});
        }

        private string GetKey(string username)
        {
            return $"UserSession_{username.ToLower()}";
        }
    }
}
