namespace FinanceApp.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }
}
