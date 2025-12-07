namespace APinI.Models.SpendBook.Requests
{
    public class UpdateAccountRequest
    {
        public string Username { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string NewAccountName { get; set; } = string.Empty;
    }
}
