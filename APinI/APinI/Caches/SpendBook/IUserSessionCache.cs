using APinI.Models.SpendBook;

namespace APinI.Caches.SpendBook
{
    public interface IUserSessionCache
    {
        UserInfo GetUserInfoByUsername(string username);
        void SetUserSessionCache(UserInfo userInfo);
    }
}
