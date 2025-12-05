namespace APinI.Models
{
    public class ApiBaseResponse<T> where T : BaseResponse
    {
        public ApiBaseResponse(T response)
        {
            Data = response;
            ErrorCode = response.ErrorCode;
            ErrorMessage = response.ErrorMessage;
        }
        public T Data { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
