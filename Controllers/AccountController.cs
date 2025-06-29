using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using TWeb.Business.Interfaces;
using TWeb.Models.ViewModels;

namespace TWeb.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountBusinessLogic _accountBusinessLogic;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountBusinessLogic accountBusinessLogic, ILogger<AccountController> logger)
        {
            _accountBusinessLogic = accountBusinessLogic;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountBusinessLogic.RegisterAsync(model);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

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

            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountBusinessLogic.LoginAsync(model);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} logged in.", model.UserName);
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserName} account locked out.", model.UserName);
                ModelState.AddModelError(string.Empty, "Account locked out.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await _accountBusinessLogic.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            return Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Home");
        }
    }
}
