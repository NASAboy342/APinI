namespace APinI.Models.SpendBook.Response
{
    public class GetTransactionResponse : BaseResponse
    {
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
