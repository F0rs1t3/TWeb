using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.Business.Interfaces;

namespace TWeb.Controllers
{
    [Authorize]
    public class CarController : BaseController
    {
        private readonly ICarBusinessLogic _carBusinessLogic;

        public CarController(ICarBusinessLogic carBusinessLogic)
        {
            _carBusinessLogic = carBusinessLogic;
        }

        public IActionResult Index()
        {
            // In future, this could redirect based on logic from business layer
            return RedirectToAction("Buy", "Cars");
        }
    }
}
