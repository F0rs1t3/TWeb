using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TWeb.Business.Interfaces;
using TWeb.Models;

namespace TWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeBusinessLogic _homeBusinessLogic;

        public HomeController(ILogger<HomeController> logger, IHomeBusinessLogic homeBusinessLogic)
        {
            _logger = logger;
            _homeBusinessLogic = homeBusinessLogic;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            return await _homeBusinessLogic.CreateAdminAsync();
        }
    }
}
