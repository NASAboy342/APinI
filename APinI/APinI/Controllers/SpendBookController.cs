using APinI.Models;
using APinI.Models.SpendBook.Requests;
using Microsoft.AspNetCore.Mvc;

namespace APinI.Controllers
{
    [ApiController]
    [Route("api/spendbook")]
    public class SpendBookController : ControllerBase
    {

        [HttpPost("create-user")]
        public async Task<ApiBaseResponse<BaseResponse>> CreateUser(CreateUserRequest req)
        {
            return new ApiBaseResponse<BaseResponse>(new BaseResponse
            {
                ErrorCode = 0,
                ErrorMessage = "User created successfully"
            });
        }


    }
}
