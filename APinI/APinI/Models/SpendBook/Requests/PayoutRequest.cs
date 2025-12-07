namespace APinI.Models.SpendBook.Requests
{
    public class PayoutRequest
    {
        public string Username { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? Remarks { get; set; }
        public int? TrackingTopicId { get; set; }
    }
}
