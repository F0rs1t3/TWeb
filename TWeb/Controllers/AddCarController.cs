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
            // You can save the data to the database here
            // For now, just redirect or return a confirmation view

            TempData["Message"] = $"Car {Make} {Model} added successfully!";
            return RedirectToAction("Index");
        }
    }
}
