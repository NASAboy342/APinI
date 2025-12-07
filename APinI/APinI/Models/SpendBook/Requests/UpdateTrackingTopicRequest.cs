using APinI.Enums.SpendBook;

namespace APinI.Models.SpendBook.Requests
{
    public class UpdateTrackingTopicRequest
    {
        public string Username { get; set; } = string.Empty;
        public int TrackingTopicId { get; set; }
        public string NewName { get; set; } = string.Empty;
        public EnumPaymentTrackingTopicStatus NewStatus { get; set; }
    }
}
