namespace APinI.Models.SpendBook.Requests
{
    public class GetTransactionRequest
    {
        public string Username { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public DateTimeOffset FromUtcDate { get; set; }
        public DateTimeOffset ToUtcDate { get; set; }
        public int? TrackingTopicId { get; set; }
    }
}
