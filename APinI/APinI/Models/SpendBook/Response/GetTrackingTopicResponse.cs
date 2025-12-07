namespace APinI.Models.SpendBook.Response
{
    public class GetTrackingTopicResponse : BaseResponse
    {
        public List<PaymentTrackingTopic> Topics { get; set; } = new List<PaymentTrackingTopic>();
    }
}
