using APinI.Models;
using APinI.Models.SpendBook;
using APinI.Models.SpendBook.Requests;
using APinI.Models.SpendBook.Response;
using System.Xml.Linq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace APinI.Repository
{
    public class SpendBookRepository : BaseRepository, ISpendBookRepository
    {
        public SpendBookRepository() : base("Data Source=.;Initial Catalog=SpendBookDb;User Id=sa;Password=1234qwer")
        {
        }

        public void CreateAccount(Account account)
        {
            GetData<BaseResponse>("[CreateAccount]", new
            {
                UserId = account.UserId,
                Name = account.Name,
                Balance = account.Balance,
                Currency = account.Currency,
                UsdRate = account.UsdRate,
                UtcCreatedOn = account.UtcCreatedOn,
                UtcLastAccessedOn = DateTimeOffset.UtcNow
            });
        }

        public void CreateTrackingTopic(PaymentTrackingTopic trackingTopic)
        {
            GetData<BaseResponse>("[CreateTrackingTopic]", new
            {
                Topic = trackingTopic.Topic,
                UserId = trackingTopic.UserId,
                UtcTargetDate = trackingTopic.UtcTargetDate,
                TargetAmount = trackingTopic.TargetAmount,
                Currency = trackingTopic.Currency,
                UtcCreatedOn = trackingTopic.UtcCreatedOn,
                Status = trackingTopic.Status
            });
        }

        public void CreateUser(CreateUserRequest req)
        {
            GetData<BaseResponse>("[CreateUser]", new
            {
                username = req.Username,
                password = req.Password,
                utcCreateOn = DateTimeOffset.UtcNow
            });
        }

        public List<Account> GetAccountsByUsername(string username)
        {
            var accounts = GetData<Account>("[GetAccountsByUsername]", new
            {
                username = username
            }).ToList();
            return accounts;
        }

        public List<PaymentTrackingTopic> GetPaymentTrackingTopicsByUsername(string username)
        {
            var trackingTopics = GetData<PaymentTrackingTopic>("[GetPaymentTrackingTopicsByUsername]", new
            {
                username = username
            }).ToList();
            return trackingTopics;
        }

        public List<Transaction> GetTransactionsByAccountIdAndDateRange(int accountId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var transactions = GetData<Transaction>("[GetTransactionsByAccountIdAndDateRange]", new
            {
                accountId = accountId,
                startDate = startDate,
                endDate = endDate
            }).ToList();
            return transactions;
        }

        public UserInfo GetUserInfoByUsername(string username)
        {
            var userInfo = GetData<UserInfo>("[GetUserInfoByUsername]", new
            {
                username = username
            }).FirstOrDefault();
            return userInfo;
        }

        public void StoreTransaction(Transaction transaction)
        {
            GetData<BaseResponse>("[StoreTransaction]", new
            {
                accountId = transaction.AccountId,
                amount = transaction.Amount,
                balanceBefore = transaction.BalanceBefore,
                balanceAfter = transaction.BalanceAfter,
                receiptUrl = transaction.ReceiptUrl,
                remarks = transaction.Remarks,
                timeStamp = transaction.TimeStamp,
                trackingTopicId = transaction.TrackingTopicId
            });
        }


        public void UpdateBalance(int accountId, decimal amount)
        {
            GetData<BaseResponse>("[UpdateBalance]", new
            {
                accountId = accountId,
                amount = amount
            });
        }
        public void UpdateAccount(UpdateAccountRequest req)
        {
            GetData<BaseResponse>("[UpdateAccount]", new
            {
                accountId = req.AccountId,
                newAccountName = req.NewAccountName,
                modifiedOn = DateTimeOffset.UtcNow
            });
        }

        public void UpdateTrackingTopic(UpdateTrackingTopicRequest req)
        {
            GetData<BaseResponse>("[UpdateTrackingTopic]", new
            {
                topicId = req.TrackingTopicId,
                newName = req.NewName,
                status = req.NewStatus.ToString(),
                modifiedOn = DateTimeOffset.UtcNow
            });
        }
    }
}
