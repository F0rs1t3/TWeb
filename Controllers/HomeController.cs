using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using TWeb.Models;

namespace TWeb.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
            
            // Creează rolul Admin dacă nu există
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            
            // Verifică dacă adminul există deja
            var adminUser = await userManager.FindByEmailAsync("admin@carlux.com");
            if (adminUser == null)
            {
                // Creează utilizatorul admin
                adminUser = new ApplicationUser
                {
                    UserName = "admin@carlux.com",
                    Email = "admin@carlux.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "CarLux"
                };
                
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    return Json(new { success = true, message = "Admin creat cu succes!" });
                }
                else
                {
                    return Json(new { success = false, errors = result.Errors });
                }
            }
            else
            {
                // Dacă există, asigură-te că are rolul de admin
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                return Json(new { success = true, message = "Admin există deja!" });
            }
        }
    }
}
