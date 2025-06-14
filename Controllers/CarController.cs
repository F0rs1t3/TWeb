using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TWeb.Data;
using TWeb.Models;
using TWeb.Services.Interfaces;

namespace TWeb.Controllers
{
    [Authorize]
    public class CarController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICarService _carService;

        public CarController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ICarService carService)
        {
            _context = context;
            _userManager = userManager;
            _carService = carService;
        }

        // List all controllers to see what we have
        public IActionResult Index()
        {
            return RedirectToAction("Buy", "Cars");
        }
    }
}
