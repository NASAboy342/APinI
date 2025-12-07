namespace APinI.Models.SpendBook.Requests
{
    public class CreateTrackingTopicRequest
    {
        public string Username { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public DateTimeOffset UtcTargetDate { get; set; }
        public decimal TargetAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
