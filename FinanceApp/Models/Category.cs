namespace FinanceApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsIncome { get; set; } // true = приход, false = разход
        public string UserId { get; set; } 
    }
}
