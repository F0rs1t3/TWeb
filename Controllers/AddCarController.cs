using Microsoft.AspNetCore.Mvc;

namespace TWeb.Controllers
{
    public class AddCarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCar(string Make, string Model, int Year, decimal Price, string Description)
        {

            TempData["Message"] = $"Car {Make} {Model} added successfully!";
            return RedirectToAction("Index");
        }
    }
}