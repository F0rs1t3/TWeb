using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.Business.Interfaces;
using TWeb.Models.ViewModels;

namespace TWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountBusinessLogic _accountLogic;

        public AccountController(IAccountBusinessLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountLogic.RegisterAsync(model);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _accountLogic.LoginAsync(model);

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);

                if (result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "Account locked out.");
                else
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await _accountLogic.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
