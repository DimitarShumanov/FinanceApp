namespace FinanceApp.DTOs
{
    public class TransactionDto
    {
        public int CategoryId { get; set; }
        public DateTime Date {  get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
