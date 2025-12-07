using APinI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace APinI.Filter.SpendBook
{
    public class SpendBookExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var baseResponse = new ApiBaseResponse<BaseResponse>(
                new BaseResponse 
                { 
                    ErrorCode = 1, 
                    ErrorMessage = context.Exception.Message 
                });

            context.Result = new JsonResult(baseResponse)
            {
                StatusCode = 200
            };
            context.ExceptionHandled = true;
        }
    }
}
