using APinI.Caches.SpendBook;
using APinI.Filter.SpendBook;
using APinI.Models;
using APinI.Models.SpendBook.Requests;
using APinI.Models.SpendBook.Response;
using APinI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers
{
    [ApiController]
    [Route("api/spendbook")]
    [ServiceFilter(typeof(SpendBookExceptionFilter))]
    public class SpendBookController : ControllerBase
    {
        private readonly ISpendBookSerivce _spendBookService;
        private readonly IUserSessionCache _userSessionCache;

        public SpendBookController(ISpendBookSerivce spendBookService, IUserSessionCache userSessionCache)
        {
            _spendBookService = spendBookService;
            _userSessionCache = userSessionCache;
        }

        [HttpPost("create-user")]
        public async Task<ApiBaseResponse<BaseResponse>> CreateUser(CreateUserRequest req)
        {
            _spendBookService.ValidateCreateUserRequest(req);
            _spendBookService.EncryptCreateUserRequestPassword(req);
            var response = _spendBookService.CreateUser(req);
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("login")]
        public async Task<ApiBaseResponse<LoginResponse>> Login(LoginRequest req)
        {
            _spendBookService.ValidateLoginRequest(req);
            _spendBookService.EncryptLoginRequestPassword(req);
            var userInfo = _spendBookService.GetUserInfo(req.Username);
            _spendBookService.ValidatePassword(req.Password, userInfo.Password);
            _userSessionCache.SetUserSessionCache(userInfo);
            var response = _spendBookService.CreateLoginResponse(req.Username);
            return new ApiBaseResponse<LoginResponse>(response);
        }

        [HttpPost("get-user-summary-status")]
        public async Task<ApiBaseResponse<GetUserSummaryStatusResponse>> GetUserSummaryStatus(GetUserSummaryStatusRequest req)
        {
            _spendBookService.ValidateGetUserSummaryStatusRequest(req);
            var accounts = _spendBookService.GetUserAccountsByUserName(req.Username);
            await _spendBookService.AssignTransactionSummayByTime(accounts);
            var trackingPaymentSummaries = await _spendBookService.GetTrackingPaymentSummary(req.Username);
            var response = new GetUserSummaryStatusResponse()
            {
                Accounts = accounts,
                TrackingPaymentSummaries = trackingPaymentSummaries,
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<GetUserSummaryStatusResponse>(response);
        }

        [HttpPost("create-account")]
        public async Task<ApiBaseResponse<BaseResponse>> CreateAccount(CreateAccountRequest req)
        {
            _spendBookService.ValidateCreateAccountRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            await _spendBookService.CreateAccount(req, userInfo);
            var response = new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("payin")]
        public async Task<ApiBaseResponse<BaseResponse>> Payin(PayinRequest req)
        {
            _spendBookService.ValidatePayinRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            var transaction = _spendBookService.ProcessPayinTransaction(req, userInfo);
            _spendBookService.StoreTransaction(transaction);
            var response = new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("payout")]
        public async Task<ApiBaseResponse<BaseResponse>> Payout(PayoutRequest req)
        {
            _spendBookService.ValidatePayoutRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            var transaction = _spendBookService.ProcessPayoutTransaction(req, userInfo);
            _spendBookService.StoreTransaction(transaction);
            var response = new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("create-tracking-topic")]
        public async Task<ApiBaseResponse<BaseResponse>> CreateTrackingTopic(CreateTrackingTopicRequest req)
        {
            _spendBookService.ValidateCreateTrackingTopicRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            _spendBookService.CreateTrackingTopic(req, userInfo);
            var response = new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("get-tracking-topic")]
        public async Task<ApiBaseResponse<GetTrackingTopicResponse>> GetTrackingTopic(GetTrackingTopicRequest req)
        {
            _spendBookService.ValidateGetTrackingTopicRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            var trackingTopics = await _spendBookService.GetTrackingTopicsByUsername(userInfo.Username);
            var response = new GetTrackingTopicResponse()
            {
                Topics = trackingTopics,
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<GetTrackingTopicResponse>(response);
        }

        [HttpPost("update-tracking-topic")]
        public async Task<ApiBaseResponse<BaseResponse>> UpdateTrackingTopic(UpdateTrackingTopicRequest req)
        {
            _spendBookService.ValidateUpdateTrackingTopicRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            _spendBookService.UpdateTrackingTopic(req, userInfo);
            var response = new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("update-account")]
        public async Task<ApiBaseResponse<BaseResponse>> UpdateAccount(UpdateAccountRequest req)
        {
            _spendBookService.ValidateUpdateAccountRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            _spendBookService.UpdateAccount(req, userInfo);
            var response = new BaseResponse()
            {
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<BaseResponse>(response);
        }

        [HttpPost("get-transaction")]
        public async Task<ApiBaseResponse<GetTransactionResponse>> GetTransaction(GetTransactionRequest req)
        {
            _spendBookService.ValidateGetTransactionRequest(req);
            var userInfo = _userSessionCache.GetUserInfoByUsername(req.Username);
            var transactions = await _spendBookService.GetTransactionsByAccountIdAndDateRange(req, userInfo);
            var response = new GetTransactionResponse()
            {
                Transactions = transactions,
                ErrorCode = 0,
                ErrorMessage = "Success"
            };
            return new ApiBaseResponse<GetTransactionResponse>(response);
        }
    }
}
