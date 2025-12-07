namespace APinI.Models.SpendBook.Requests
{
    public class CreateAccountRequest
    {
        public string Username { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
        public string Currency { get; set; }
    }
}
