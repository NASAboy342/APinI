using APinI.Enums.SpendBook;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Xml.Linq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace APinI.Models.SpendBook.Response
{
    public class GetUserSummaryStatusResponse : BaseResponse
    {
        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<TrackingPaymentSummary> TrackingPaymentSummaries { get; set; } = new List<TrackingPaymentSummary>();
    }

    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public decimal UsdRate { get; set; }
        public DateTimeOffset? UtcCreatedOn { get; set; }
        public DateTimeOffset? UtcModifiedOn { get; set; }
        public DateTimeOffset? UtcLastAccessedOn { get; set; }

        public TransactionSummaryByTime DailyPayIn { get; set; }
        public TransactionSummaryByTime DailyPayOut { get; set; }
        public TransactionSummaryByTime WeeklyPayIn { get; set; }
        public TransactionSummaryByTime WeeklyPayOut { get; set; }
        public TransactionSummaryByTime MonthlyPayIn { get; set; }
        public TransactionSummaryByTime MonthlyPayOut { get; set; }
        public TransactionSummaryByTime YearlyPayIn { get; set; }
        public TransactionSummaryByTime YearlyPayOut { get; set; }
    }

    public class TrackingPaymentSummary
    {
        public int TrackingTopicId { get; set; }
        public string TrackingTopicName { get; set; }
        public EnumPaymentTrackingTopicStatus StatusCode { get; set; }
        public string StatusName { get; set; }
    }

    public class TransactionSummaryByTime
    {
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public DateTimeOffset UtcStartDate { get; set; }
        public DateTimeOffset UtcEndDate { get; set; }
    }
}
