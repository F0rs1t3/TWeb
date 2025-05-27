using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager)
            : base(userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View(); // Shows profile details
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Step 1: Check current password
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError("Password", "Incorrect password. Please try again.");
                return View(model);
            }

            // Step 2: Update profile fields
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.UserName;

            var profileUpdateResult = await _userManager.UpdateAsync(user);

            if (!profileUpdateResult.Succeeded)
            {
                foreach (var error in profileUpdateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            // Step 3: Handle password change if requested
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

    }
}
