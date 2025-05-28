using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TWeb.Models;
using TWeb.Models.ViewModels;
using BusinessLogic;

namespace TWeb.Controllers
{
    public class CarController : BaseController
    {
        private readonly CarService _carService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(CarService carService, IWebHostEnvironment webHostEnvironment)
        {
            _carService = carService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            ViewBag.IsAdmin = User.IsInRole("Administrator");
            return View(cars);
        }

        [Authorize]
        public async Task<IActionResult> MyCars()
        {
            var userId = GetCurrentUserId();
            var cars = await _carService.GetCarsByOwnerAsync(userId);
            return View(cars);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            
            ViewBag.IsOwner = car.OwnerId == GetCurrentUserId();
            ViewBag.IsAdmin = User.IsInRole("Administrator");
            return View(car);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddCarViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = GetCurrentUserId();
                
                var car = new Car
                {
                    Brand = viewModel.Brand,
                    Model = viewModel.Model,
                    Year = viewModel.Year,
                    Mileage = viewModel.Mileage,
                    OwnerId = userId,
                    CreatedAt = DateTime.Now
                };
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cars");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Photo.CopyToAsync(fileStream);
                    }
                    
                    car.PhotoPath = "/images/cars/" + uniqueFileName;
                }

                await _carService.AddCarAsync(car);
                TempData["SuccessMessage"] = "Car added successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            // Only owner or admin can edit
            if (car.OwnerId != GetCurrentUserId() && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            var viewModel = new AddCarViewModel
            {
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Mileage = car.Mileage
            };

            ViewBag.CarId = car.Id;
            ViewBag.CurrentPhoto = car.PhotoPath;
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddCarViewModel viewModel)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            // Only owner or admin can edit
            if (car.OwnerId != GetCurrentUserId() && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                car.Brand = viewModel.Brand;
                car.Model = viewModel.Model;
                car.Year = viewModel.Year;
                car.Mileage = viewModel.Mileage;

                // Handle photo upload
                if (viewModel.Photo != null && viewModel.Photo.Length > 0)
                {
                    // Delete old photo if exists
                    if (!string.IsNullOrEmpty(car.PhotoPath))
                    {
                        var oldPhotoPath = Path.Combine(_webHostEnvironment.WebRootPath, car.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPhotoPath))
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cars");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.Photo.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.Photo.CopyToAsync(fileStream);
                    }
                    
                    car.PhotoPath = "/images/cars/" + uniqueFileName;
                }

                await _carService.UpdateCarAsync(car);
                TempData["SuccessMessage"] = "Car updated successfully!";
                return RedirectToAction(nameof(Details), new { id = car.Id });
            }

            ViewBag.CarId = id;
            ViewBag.CurrentPhoto = car.PhotoPath;
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            // Only owner or admin can delete
            if (car.OwnerId != GetCurrentUserId() && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            // Delete photo file if exists
            if (!string.IsNullOrEmpty(car.PhotoPath))
            {
                var photoPath = Path.Combine(_webHostEnvironment.WebRootPath, car.PhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
            }

            await _carService.DeleteCarAsync(id);
            
            if (User.IsInRole("Administrator"))
            {
                TempData["SuccessMessage"] = "Car deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["SuccessMessage"] = "Car deleted successfully!";
                return RedirectToAction(nameof(MyCars));
            }
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AdminPanel()
        {
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }
    }
}
