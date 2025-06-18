using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Add user to the default role
                    await _userManager.AddToRoleAsync(user, "User");

                    // Add claims for first name and last name
                    var claims = new List<Claim>
                    {
                        new Claim("FirstName", user.FirstName ?? string.Empty),
                        new Claim("LastName", user.LastName ?? string.Empty)
                    };
                    await _userManager.AddClaimsAsync(user, claims);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
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
                // Use UserName for login (not email)
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserName} logged in.", model.UserName);

                    // Update claims after login
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        var existingClaims = await _userManager.GetClaimsAsync(user);
                        
                        // Add FirstName claim if it doesn't exist
                        if (!existingClaims.Any(c => c.Type == "FirstName"))
                        {
                            await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));
                        }
                        
                        // Add LastName claim if it doesn't exist
                        if (!existingClaims.Any(c => c.Type == "LastName"))
                        {
                            await _userManager.AddClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
                        }
                    }

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
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}