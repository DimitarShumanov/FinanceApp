namespace FinanceApp.Auth
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string? Currency { get; set; } = "BGN";
    }
}
