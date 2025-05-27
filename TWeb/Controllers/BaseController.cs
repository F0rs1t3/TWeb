using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TWeb.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TWeb.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (User.Identity.IsAuthenticated)
            {
                var userTask = _userManager.GetUserAsync(User);
                userTask.Wait(); // Wait here because OnActionExecuting is not async
                var user = userTask.Result;

                if (user != null)
                {
                    ViewBag.FirstName = user.FirstName;
                    ViewBag.LastName = user.LastName;
                    ViewBag.UserName = user.UserName;
                    ViewBag.Email = user.Email;
                }
            }
        }
    }
}
