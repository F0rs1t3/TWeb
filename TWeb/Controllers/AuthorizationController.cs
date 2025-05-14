using Microsoft.AspNetCore.Mvc;

namespace TWeb.Controllers
{
    public class AuthorizationController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
