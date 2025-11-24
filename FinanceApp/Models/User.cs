using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace FinanceApp.Models
{
    public class User: IdentityUser
    {
        public string DisplayName { get; set; }
        public string Currency { get; set; } = "BGN";
    }
}
