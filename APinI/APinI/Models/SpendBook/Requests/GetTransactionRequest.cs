namespace APinI.Models.SpendBook.Requests
{
    public class GetTransactionRequest
    {
        public string Username { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public DateTimeOffset FromUtcDate { get; set; } = DateTimeOffset.UtcNow.AddMonths(-1);
        public DateTimeOffset ToUtcDate { get; set; } = DateTimeOffset.UtcNow;
        public int? TrackingTopicId { get; set; } = 0;
    }
}
