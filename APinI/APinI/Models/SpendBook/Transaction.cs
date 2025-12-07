using APinI.Models.SpendBook.Response;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APinI.Models.SpendBook
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? Remarks { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public int? TrackingTopicId { get; set; }
    }
}