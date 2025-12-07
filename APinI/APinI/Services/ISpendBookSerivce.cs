using APinI.Models;
using APinI.Models.SpendBook;
using APinI.Models.SpendBook.Requests;
using APinI.Models.SpendBook.Response;

namespace APinI.Services
{
    public interface ISpendBookSerivce
    {
        Task<List<TrackingPaymentSummary>> GetTrackingPaymentSummary(string username);
        Task AssignTransactionSummayByTime(List<Account> accounts);
        LoginResponse CreateLoginResponse(string username);
        BaseResponse CreateUser(CreateUserRequest req);
        void EncryptCreateUserRequestPassword(CreateUserRequest req);
        void EncryptLoginRequestPassword(LoginRequest req);
        List<Account> GetUserAccountsByUserName(string username);
        UserInfo GetUserInfo(string username);
        void ValidateCreateUserRequest(CreateUserRequest req);
        void ValidateGetUserSummaryStatusRequest(GetUserSummaryStatusRequest req);
        void ValidateLoginRequest(LoginRequest req);
        void ValidatePassword(string encryptedPassword1, string encryptedPassword2);
        void ValidateCreateAccountRequest(CreateAccountRequest req);
        Task CreateAccount(CreateAccountRequest req, UserInfo userInfo);
        void ValidatePayinRequest(PayinRequest req);
        Transaction ProcessPayinTransaction(PayinRequest req, UserInfo userInfo);
        void StoreTransaction(Transaction transaction);
        void ValidatePayoutRequest(PayoutRequest req);
        Transaction ProcessPayoutTransaction(PayoutRequest req, UserInfo userInfo);
        void ValidateCreateTrackingTopicRequest(CreateTrackingTopicRequest req);
        void CreateTrackingTopic(CreateTrackingTopicRequest req, UserInfo userInfo);
        void ValidateGetTrackingTopicRequest(GetTrackingTopicRequest req);
        Task<List<PaymentTrackingTopic>> GetTrackingTopicsByUsername(string username);
        void ValidateUpdateTrackingTopicRequest(UpdateTrackingTopicRequest req);
        void UpdateTrackingTopic(UpdateTrackingTopicRequest req, UserInfo userInfo);
        void ValidateUpdateAccountRequest(UpdateAccountRequest req);
        void UpdateAccount(UpdateAccountRequest req, UserInfo userInfo);
        void ValidateGetTransactionRequest(GetTransactionRequest req);
        Task<List<Transaction>> GetTransactionsByAccountIdAndDateRange(GetTransactionRequest req, UserInfo userInfo);
    }
}
