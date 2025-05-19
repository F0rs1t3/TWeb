using Microsoft.AspNetCore.Mvc;

namespace TWeb.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

