using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Business.Interfaces
{
    public interface IProfileBusinessLogic
    {
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
        Task<EditProfileViewModel?> GetProfileEditModelAsync(ClaimsPrincipal userPrincipal);
        Task<IdentityResult> UpdateProfileAsync(ClaimsPrincipal userPrincipal, EditProfileViewModel model);
    }
}
