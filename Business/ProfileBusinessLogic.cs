using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TWeb.Business.Interfaces;
using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Business
{
    public class ProfileBusinessLogic : IProfileBusinessLogic
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProfileBusinessLogic> _logger;

        public ProfileBusinessLogic(UserManager<ApplicationUser> userManager, ILogger<ProfileBusinessLogic> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            return await _userManager.GetUserAsync(userPrincipal);
        }

        public async Task<EditProfileViewModel?> GetProfileEditModelAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null)
                return null;

            return new EditProfileViewModel
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty
            };
        }

        public async Task<IdentityResult> UpdateProfileAsync(ClaimsPrincipal userPrincipal, EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.UserName;

            return await _userManager.UpdateAsync(user);
        }
    }
}
