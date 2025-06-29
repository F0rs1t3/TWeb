using Microsoft.AspNetCore.Identity;
using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Business.Interfaces
{
    public interface IAccountBusinessLogic
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}
