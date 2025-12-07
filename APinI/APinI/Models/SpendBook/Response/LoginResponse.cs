namespace APinI.Models.SpendBook.Response
{
    public class LoginResponse : BaseResponse
    {
        public string Username { get; set; }
        public DateTimeOffset UtcCreateOn { get; set; }
    }
}
