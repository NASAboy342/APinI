using APinI.Enums.SpendBook;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace APinI.Models.SpendBook
{
    public class PaymentTrackingTopic
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset UtcTargetDate { get; set; }
        public decimal TargetAmount { get; set; }
        public string Currency { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public DateTimeOffset UtcModifiedOn { get; set; }
        public string Status { get; set; }
        public EnumPaymentTrackingTopicStatus StatusCode => Enum.TryParse<EnumPaymentTrackingTopicStatus>(Status, out var status) ? status : EnumPaymentTrackingTopicStatus.Unknown;
        public void SetStatus(EnumPaymentTrackingTopicStatus status)
        {
            Status = status.ToString();
        }
    }
}