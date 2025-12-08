using APinI.Caches.SpendBook;
using APinI.Enums.SpendBook;
using APinI.Models;
using APinI.Models.SpendBook;
using APinI.Models.SpendBook.Requests;
using APinI.Models.SpendBook.Response;
using APinI.Repository;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Policy;

namespace APinI.Services
{
    public class SpendBookSerivce : ISpendBookSerivce
    {
        private readonly ISpendBookRepository _spendBookRepository;
        private readonly IUserSessionCache _userSessionCache;
        private readonly Dictionary<string, Currency> _supportedCurrencies = new Dictionary<string, Currency>
        {
            { "USD", new Currency{ Code = "USD", UsdRate = 1 } },
            { "KHR", new Currency{ Code = "KHR", UsdRate = 4000 } },
        };

        public SpendBookSerivce(ISpendBookRepository spendBookRepository, IUserSessionCache userSessionCache)
        {
            _spendBookRepository = spendBookRepository;
            _userSessionCache = userSessionCache;
        }

        public LoginResponse CreateLoginResponse(string username)
        {
            var userInfo = _userSessionCache.GetUserInfoByUsername(username);
            var loginResponse = new LoginResponse
            {
                Username = userInfo.Username,
                UtcCreateOn = userInfo.UtcCreatedOn,
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return loginResponse;
        }

        public BaseResponse CreateUser(CreateUserRequest req)
        {
            _spendBookRepository.CreateUser(req);
            return new BaseResponse
            {
                ErrorCode = 0,
                ErrorMessage = "User created successfully"
            };
        }

        public void EncryptCreateUserRequestPassword(CreateUserRequest req)
        {
            req.Password = EncryptString(req.Password);
        }

        private static string EncryptString(string stringToEncrypt)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(stringToEncrypt));
        }

        public void EncryptLoginRequestPassword(LoginRequest req)
        {
            req.Password = EncryptString(req.Password);
        }

        public UserInfo GetUserInfo(string username)
        {
            var userInfo = _spendBookRepository.GetUserInfoByUsername(username);
            if (userInfo == null)
                throw new Exception($"userInfo of {username} is not found");
            return userInfo;
        }

        public void ValidateCreateUserRequest(CreateUserRequest req)
        {
            if(string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            if(req.Username.Contains(" "))
            {
                throw new ArgumentException("Username cannot contain spaces");
            }
            if (string.IsNullOrEmpty(req.Password))
            {
                throw new ArgumentException("Password cannot be empty");
            }
            if(req.Password.Contains(" "))
            {
                throw new ArgumentException("Password cannot contain spaces");
            }
            if (_spendBookRepository.GetUserInfoByUsername(req.Username) != null)
            {
                throw new ArgumentException("Username already exists");
            }
        }

        public void ValidateLoginRequest(LoginRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            if (string.IsNullOrEmpty(req.Password))
            {
                throw new ArgumentException("Password cannot be empty");
            }
        }

        public void ValidatePassword(string encryptedPassword1, string encryptedPassword2)
        {
            if(!encryptedPassword2.Equals(encryptedPassword1))
            {
                throw new ArgumentException("Invalid password");
            }
        }


        public async Task AssignTransactionSummayByTime(List<Account> accounts)
        {
            foreach(var account in accounts)
            {
                account.DailyPayOut = GetDailyPayoutSummary(account.Id);
                account.DailyPayIn = GetDailyPayinSummary(account.Id);
                account.WeeklyPayIn = GetWeeklyPayinSummary(account.Id);
                account.WeeklyPayOut = GetWeeklyPayoutSummary(account.Id);
                account.MonthlyPayIn = GetMonthlyPayinSummary(account.Id);
                account.MonthlyPayOut = GetMonthlyPayoutSummary(account.Id);
                account.YearlyPayIn = GetYearlyPayinSummary(account.Id);
                account.YearlyPayOut = GetYearlyPayoutSummary(account.Id);
            }
        }

        private TransactionSummaryByTime GetYearlyPayoutSummary(int id)
        {
            var startOfThisYear = new DateTimeOffset(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var endOfThisYear = new DateTimeOffset(DateTime.UtcNow.Year, 12, 31, 23, 59, 59, TimeSpan.Zero);
            return GetPayoutTransactionSummaryByDateRange(id, startOfThisYear, endOfThisYear);
        }

        private TransactionSummaryByTime GetPayoutTransactionSummaryByDateRange(int id, DateTimeOffset startOfThisYear, DateTimeOffset endOfThisYear)
        {
            var transactions = _spendBookRepository.GetTransactionsByAccountIdAndDateRange(id, startOfThisYear, endOfThisYear);
            var payoutTransactions = transactions.Where(t => t.Amount < 0).ToList();
            var transactionSummary = new TransactionSummaryByTime
            {
                TotalAmount = payoutTransactions.Sum(t => t.Amount),
                TransactionCount = payoutTransactions.Count,
                UtcStartDate = startOfThisYear,
                UtcEndDate = endOfThisYear
            };
            return transactionSummary;
        }

        private TransactionSummaryByTime GetYearlyPayinSummary(int id)
        {
            var startOfThisYear = new DateTimeOffset(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var endOfThisYear = new DateTimeOffset(DateTime.UtcNow.Year, 12, 31, 23, 59, 59, TimeSpan.Zero);
            return GetPayinTransactionSummaryByDateRange(id, startOfThisYear, endOfThisYear);
        }

        private TransactionSummaryByTime GetPayinTransactionSummaryByDateRange(int id, DateTimeOffset startOfThisYear, DateTimeOffset endOfThisYear)
        {
            var transactions = _spendBookRepository.GetTransactionsByAccountIdAndDateRange(id, startOfThisYear, endOfThisYear);
            var payoutTransactions = transactions.Where(t => t.Amount >= 0).ToList();
            var transactionSummary = new TransactionSummaryByTime
            {
                TotalAmount = payoutTransactions.Sum(t => t.Amount),
                TransactionCount = payoutTransactions.Count,
                UtcStartDate = startOfThisYear,
                UtcEndDate = endOfThisYear
            };
            return transactionSummary;
        }

        private TransactionSummaryByTime GetMonthlyPayoutSummary(int id)
        {
            var startOfThisMonth = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var endOfThisMonth = startOfThisMonth.AddMonths(1).AddSeconds(-1);
            return GetPayoutTransactionSummaryByDateRange(id, startOfThisMonth, endOfThisMonth);
        }

        private TransactionSummaryByTime GetMonthlyPayinSummary(int id)
        {
            var startOfThisMonth = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var endOfThisMonth = startOfThisMonth.AddMonths(1).AddSeconds(-1);
            return GetPayinTransactionSummaryByDateRange(id, startOfThisMonth, endOfThisMonth);
        }

        private TransactionSummaryByTime GetWeeklyPayoutSummary(int id)
        {
            var startOfThisWeek = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, TimeSpan.Zero).AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var endOfThisWeek = startOfThisWeek.AddDays(7).AddSeconds(-1);
            return GetPayoutTransactionSummaryByDateRange(id, startOfThisWeek, endOfThisWeek);
        }

        private TransactionSummaryByTime GetWeeklyPayinSummary(int id)
        {
            var startOfThisWeek = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, TimeSpan.Zero).AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var endOfThisWeek = startOfThisWeek.AddDays(7).AddSeconds(-1);
            return GetPayinTransactionSummaryByDateRange(id, startOfThisWeek, endOfThisWeek);
        }

        private TransactionSummaryByTime GetDailyPayinSummary(int id)
        {
            var startOfToday = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, TimeSpan.Zero);
            var endOfToday = startOfToday.AddDays(1).AddSeconds(-1);
            return GetPayinTransactionSummaryByDateRange(id, startOfToday, endOfToday);
        }

        private TransactionSummaryByTime GetDailyPayoutSummary(int id)
        {
            var startOfToday = new DateTimeOffset(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, TimeSpan.Zero);
            var endOfToday = startOfToday.AddDays(1).AddSeconds(-1);
            return GetPayoutTransactionSummaryByDateRange(id, startOfToday, endOfToday);
        }

        public List<Account> GetUserAccountsByUserName(string username)
        {
            var accounts = _spendBookRepository.GetAccountsByUsername(username);
            if (accounts == null) return new List<Account>();
            return accounts;
        }

        public void ValidateGetUserSummaryStatusRequest(GetUserSummaryStatusRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            if (_userSessionCache.GetUserInfoByUsername(req.Username) == null)
            {
                throw new ArgumentException("User is not logged in");
            }
        }

        public async Task<List<TrackingPaymentSummary>> GetTrackingPaymentSummary(string username)
        {
            var paymentTrackingTopics = _spendBookRepository.GetPaymentTrackingTopicsByUsername(username);
            var trackingPaymentSummaries = paymentTrackingTopics.Select(pt => new TrackingPaymentSummary
            {
                TrackingTopicId = pt.Id,
                TrackingTopicName = pt.Topic,
                StatusCode = pt.StatusCode,
                StatusName = pt.Status
            }).ToList();
            return trackingPaymentSummaries;
        }

        public void ValidateCreateAccountRequest(CreateAccountRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            if (string.IsNullOrEmpty(req.AccountName))
            {
                throw new ArgumentException("Account name is empty");
            }
            if (!_supportedCurrencies.TryGetValue(req.Currency, out var currency))
            {
                throw new ArgumentException("Currency is not supported");
            }
            if (req.InitialBalance < 0)
            {
                throw new ArgumentException("Initial balance cannot be negative");
            }
        }

        public async Task CreateAccount(CreateAccountRequest req, UserInfo userInfo)
        {
            var account = new Account
            {
                UserId = userInfo.Id,
                Name = req.AccountName,
                Balance = req.InitialBalance,
                Currency = req.Currency,
                UsdRate = _supportedCurrencies[req.Currency].UsdRate,
                UtcCreatedOn = DateTimeOffset.UtcNow,
                UtcLastAccessedOn = DateTimeOffset.UtcNow
            };
            _spendBookRepository.CreateAccount(account);
        }

        public void ValidatePayinRequest(PayinRequest req)
        {
            if(string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            if(GetUserAccountsByUserName(req.Username).FirstOrDefault(a => a.Id == req.AccountId) == null)
            {
                throw new ArgumentException("Account is not found for the user");
            }
            if(req.Amount <= 0)
            {
                throw new ArgumentException("Payin amount must be greater than zero");
            }
            if(!string.IsNullOrEmpty(req.ReceiptUrl) && Uri.IsWellFormedUriString(req.ReceiptUrl, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Receipt URL is not valid");
            }
            if(req.TrackingTopicId == 0 || _spendBookRepository.GetPaymentTrackingTopicsByUsername(req.Username).Where(t => t.Id == req.TrackingTopicId).FirstOrDefault() == null)
            {
                throw new ArgumentException("Tracking topic is not found for the user");
            }
        }

        public Transaction ProcessPayinTransaction(PayinRequest req, UserInfo userInfo)
        {
            var currentBalance = GetUserAccountsByUserName(req.Username).First(a => a.Id == req.AccountId).Balance;
            _spendBookRepository.UpdateBalance(req.AccountId, req.Amount);
            var balanceAfter = GetUserAccountsByUserName(req.Username).First(a => a.Id == req.AccountId).Balance;

            var transaction = new Transaction()
            {
                AccountId = req.AccountId,
                Amount = req.Amount,
                BalanceBefore = currentBalance,
                ReceiptUrl = req.ReceiptUrl,
                Remarks = req.Remarks,
                TimeStamp = DateTimeOffset.UtcNow,
                TrackingTopicId = req.TrackingTopicId,
                BalanceAfter = balanceAfter,
            };
            return transaction;
        }

        public void StoreTransaction(Transaction transaction)
        {
            _spendBookRepository.StoreTransaction(transaction);
        }

        public void ValidatePayoutRequest(PayoutRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            if (GetUserAccountsByUserName(req.Username).FirstOrDefault(a => a.Id == req.AccountId) == null)
            {
                throw new ArgumentException("Account is not found for the user");
            }
            if (req.Amount <= 0)
            {
                throw new ArgumentException("Payin amount must be greater than zero");
            }
            if (!string.IsNullOrEmpty(req.ReceiptUrl) && Uri.IsWellFormedUriString(req.ReceiptUrl, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Receipt URL is not valid");
            }
            if (req.TrackingTopicId == 0 || _spendBookRepository.GetPaymentTrackingTopicsByUsername(req.Username).Where(t => t.Id == req.TrackingTopicId).FirstOrDefault() == null)
            {
                throw new ArgumentException("Tracking topic is not found for the user");
            }
        }

        public Transaction ProcessPayoutTransaction(PayoutRequest req, UserInfo userInfo)
        {
            var amount = -1 * req.Amount;
            var currentBalance = GetUserAccountsByUserName(req.Username).First(a => a.Id == req.AccountId).Balance;
            _spendBookRepository.UpdateBalance(req.AccountId, amount);
            var balanceAfter = GetUserAccountsByUserName(req.Username).First(a => a.Id == req.AccountId).Balance;

            var transaction = new Transaction()
            {
                AccountId = req.AccountId,
                Amount = amount,
                BalanceBefore = currentBalance,
                ReceiptUrl = req.ReceiptUrl,
                Remarks = req.Remarks,
                TimeStamp = DateTimeOffset.UtcNow,
                TrackingTopicId = req.TrackingTopicId,
                BalanceAfter = balanceAfter,
            };
            return transaction;
        }

        public void ValidateCreateTrackingTopicRequest(CreateTrackingTopicRequest req)
        {
            if(string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            req.TopicName = req.TopicName.Trim();
            if (string.IsNullOrEmpty(req.TopicName))
            {
                throw new ArgumentException("Topic name cannot be empty");
            }
            if (!_supportedCurrencies.TryGetValue(req.Currency, out var currency))
            {
                throw new ArgumentException("Currency is not supported");
            }

            var existingTopic = _spendBookRepository.GetPaymentTrackingTopicsByUsername(req.Username).
                FirstOrDefault(t =>
                t.Topic.Equals(req.TopicName, StringComparison.OrdinalIgnoreCase) && 
                t.StatusCode == EnumPaymentTrackingTopicStatus.Active);

            if (existingTopic != null)
            {
                throw new Exception("The same topic is already in active");
            }
        }

        public void CreateTrackingTopic(CreateTrackingTopicRequest req, UserInfo userInfo)
        {
            var trackingTopic = new PaymentTrackingTopic
            {
                Topic = req.TopicName.Trim(),
                UserId = userInfo.Id,
                UtcTargetDate = req.UtcTargetDate,
                TargetAmount = req.TargetAmount,
                Currency = req.Currency,
                UtcCreatedOn = DateTimeOffset.UtcNow
            };
            trackingTopic.SetStatus(EnumPaymentTrackingTopicStatus.Active);

            _spendBookRepository.CreateTrackingTopic(trackingTopic);
        }

        public void ValidateGetTrackingTopicRequest(GetTrackingTopicRequest req)
        {
            if(string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
        }

        public async Task<List<PaymentTrackingTopic>> GetTrackingTopicsByUsername(string username)
        {
            var trackingTopics = _spendBookRepository.GetPaymentTrackingTopicsByUsername(username);
            return trackingTopics;
        }

        public void ValidateUpdateTrackingTopicRequest(UpdateTrackingTopicRequest req)
        {
            if (string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            var existingTrackingTopic = _spendBookRepository.GetPaymentTrackingTopicsByUsername(req.Username).Where(t => t.Id == req.TrackingTopicId).FirstOrDefault();
            if (existingTrackingTopic == null)
            {
                throw new ArgumentException("Tracking topic is not found for the user");
            }
            if (string.IsNullOrEmpty(req.NewName))
            {
                req.NewName = existingTrackingTopic.Topic;
            }
        }

        public void UpdateTrackingTopic(UpdateTrackingTopicRequest req, UserInfo userInfo)
        {
            _spendBookRepository.UpdateTrackingTopic(req);
        }

        public void ValidateUpdateAccountRequest(UpdateAccountRequest req)
        {
            if(string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            var existingAccount = GetUserAccountsByUserName(req.Username).Where(a => a.Id == req.AccountId).FirstOrDefault();
            if (existingAccount == null)
            {
                throw new ArgumentException("Account is not found for the user");
            }
            if(string.IsNullOrEmpty(req.NewAccountName))
            {
                req.NewAccountName = existingAccount.Name;
            }
        }

        public void UpdateAccount(UpdateAccountRequest req, UserInfo userInfo)
        {
            _spendBookRepository.UpdateAccount(req);
        }

        public void ValidateGetTransactionRequest(GetTransactionRequest req)
        {
            if(string.IsNullOrEmpty(req.Username))
            {
                throw new ArgumentException("Username cannot be empty");
            }
            var existingAccount = GetUserAccountsByUserName(req.Username).Where(a => a.Id == req.AccountId).FirstOrDefault();
            if (existingAccount == null)
            {
                throw new ArgumentException("Account is not found for the user");
            }
            if (req.FromUtcDate > req.ToUtcDate)
            {
                throw new ArgumentException("FromUtcDate cannot be greater than ToUtcDate");
            }
        }

        public async Task<List<Transaction>> GetTransactionsByAccountIdAndDateRange(GetTransactionRequest req, UserInfo userInfo)
        {
            var transactions = _spendBookRepository.GetTransactionsByAccountIdAndDateRange(req.AccountId, req.FromUtcDate, req.ToUtcDate);
            transactions = transactions.Where(t => req.TrackingTopicId == 0 || t.TrackingTopicId == req.TrackingTopicId).OrderByDescending(t => t.TimeStamp).ToList();
            return transactions;
        }
    }
}
