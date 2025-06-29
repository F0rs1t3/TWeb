using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.Business.Interfaces;
using TWeb.Models.ViewModels;

namespace TWeb.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileBusinessLogic _profileBusinessLogic;

        public ProfileController(IProfileBusinessLogic profileBusinessLogic)
        {
            _profileBusinessLogic = profileBusinessLogic;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _profileBusinessLogic.GetCurrentUserAsync(User);
            if (user == null)
                return NotFound();

            return View(user);
        }

        public async Task<IActionResult> Edit()
        {
            var model = await _profileBusinessLogic.GetProfileEditModelAsync(User);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _profileBusinessLogic.UpdateProfileAsync(User, model);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}
