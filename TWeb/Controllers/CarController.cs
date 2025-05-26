using Microsoft.AspNetCore.Mvc;
using BusinessLogic;
using BusinessLogic.Models;

namespace TWeb.Controllers
{
    public class CarController : Controller
    {
        private readonly CarService _carService;

        public CarController(CarService carService)
        {
            _carService = carService;
        }

        public IActionResult Index()
        {
            var cars = _carService.GetAllCars();
            return View(cars);
        }

        public IActionResult Details(int id)
        {
            var cars = _carService.GetAllCars();
            var car = cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Car car)
        {
            if (ModelState.IsValid)
            {
                // For now, just redirect since the service doesn't have AddCar method
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cars = _carService.GetAllCars();
            var car = cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost]
        public IActionResult Edit(Car car)
        {
            if (ModelState.IsValid)
            {
                // For now, just redirect since the service doesn't have UpdateCar method
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cars = _carService.GetAllCars();
            var car = cars.FirstOrDefault(c => c.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // For now, just redirect since the service doesn't have DeleteCar method
            return RedirectToAction(nameof(Index));
        }
    }
}
