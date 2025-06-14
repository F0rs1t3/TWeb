using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TWeb.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        protected string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        protected bool IsUserLoggedIn()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }

        protected string GetCurrentUserName()
        {
            return User.Identity?.Name ?? string.Empty;
        }
    }
}
