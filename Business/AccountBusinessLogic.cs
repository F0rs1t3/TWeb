using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TWeb.Business.Interfaces;
using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Business
{
    public class AccountBusinessLogic : IAccountBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountBusinessLogic> _logger;

        public AccountBusinessLogic(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountBusinessLogic> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
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
                await _userManager.AddToRoleAsync(user, "User");

                var claims = new List<Claim>
                {
                    new Claim("FirstName", user.FirstName ?? ""),
                    new Claim("LastName", user.LastName ?? "")
                };

                await _userManager.AddClaimsAsync(user, claims);
                await _signInManager.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("User created and signed in.");
            }

            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    var claims = await _userManager.GetClaimsAsync(user);
                    if (!claims.Any(c => c.Type == "FirstName"))
                        await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));

                    if (!claims.Any(c => c.Type == "LastName"))
                        await _userManager.AddClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
                }

                _logger.LogInformation("User logged in.");
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }
    }
}
