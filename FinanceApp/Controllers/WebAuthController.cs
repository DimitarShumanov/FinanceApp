using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Controllers
{
    public class WebAuthController : Controller
    {
        [HttpGet("/login")]
        public IActionResult Login() => View();

        [HttpGet("/register")]
        public IActionResult Register() => View();
    }
}
