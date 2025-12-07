using APinI.Models.SpendBook;
using APinI.Models.SpendBook.Requests;
using APinI.Models.SpendBook.Response;

namespace APinI.Repository
{
    public interface ISpendBookRepository
    {
        void CreateAccount(Account account);
        void CreateTrackingTopic(PaymentTrackingTopic trackingTopic);
        void CreateUser(CreateUserRequest req);
        List<Account> GetAccountsByUsername(string username);
        List<PaymentTrackingTopic> GetPaymentTrackingTopicsByUsername(string username);
        List<Transaction> GetTransactionsByAccountIdAndDateRange(int accountId, DateTimeOffset startDate, DateTimeOffset endDate);
        UserInfo GetUserInfoByUsername(string username);
        void StoreTransaction(Transaction transaction);
        void UpdateAccount(UpdateAccountRequest req);
        void UpdateBalance(int accountId, decimal amount);
        void UpdateTrackingTopic(UpdateTrackingTopicRequest req);
    }
}
