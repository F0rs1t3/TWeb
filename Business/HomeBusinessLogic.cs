using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TWeb.Business.Interfaces;
using TWeb.Models;

namespace TWeb.Business
{
    public class HomeBusinessLogic : IHomeBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeBusinessLogic(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<JsonResult> CreateAdminAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await _userManager.FindByEmailAsync("admin@carlux.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@carlux.com",
                    Email = "admin@carlux.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "CarLux"
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    return new JsonResult(new { success = true, message = "Admin creat cu succes!" });
                }
                else
                {
                    return new JsonResult(new { success = false, errors = result.Errors });
                }
            }
            else
            {
                if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
                return new JsonResult(new { success = true, message = "Admin există deja!" });
            }
        }
    }
}
