using FinanceApp.Models;

namespace FinanceApp.Services
{
    public interface IJwtTokenService
    {
        string CreateToken(User user, IList<string> roles);
    }

}
