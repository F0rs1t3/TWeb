using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TWeb.Business.Interfaces;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Services.Interfaces;

namespace TWeb.Controllers
{
    [Authorize]
    public class CarsController : BaseController
    {
        private readonly ICarsBusinessLogic _carsBusinessLogic;
        private readonly ICarBusinessLogic _carBusinessLogic;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICarService _carService;

        public CarsController(
            ICarsBusinessLogic carsBusinessLogic,
            ICarBusinessLogic carBusinessLogic,
            UserManager<ApplicationUser> userManager,
            ICarService carService)
        {
            _carsBusinessLogic = carsBusinessLogic;
            _carBusinessLogic = carBusinessLogic;
            _userManager = userManager;
            _carService = carService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Buy(string? search, string? brand, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var cars = await _carService.GetCarsForSaleAsync(search, brand, minPrice, maxPrice);
                var brands = await _carService.GetAvailableBrandsAsync();

                ViewBag.Brands = brands;
                ViewBag.Search = search;
                ViewBag.Brand = brand;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;

                return View(cars);
            }
            catch
            {
                TempData["Error"] = "An error occurred while loading cars for sale.";
                return View(new List<Car>());
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Rental(string? search, string? brand, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                var cars = await _carService.GetCarsForRentalAsync(search, brand, minPrice, maxPrice);
                var brands = await _carService.GetRentalBrandsAsync();

                ViewBag.Brands = brands;
                ViewBag.Search = search;
                ViewBag.Brand = brand;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;

                return View(cars);
            }
            catch
            {
                TempData["Error"] = "An error occurred while loading cars for rental.";
                return View(new List<Car>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCarViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var car = await _carService.CreateCarAsync(viewModel, user!.Id);

                TempData["Success"] = "Car listing created successfully!";
                return RedirectToAction(nameof(Details), new { id = car.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
            catch
            {
                TempData["Error"] = "An error occurred while creating the car listing.";
                return View(viewModel);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                if (car == null)
                    return NotFound();

                return View(car);
            }
            catch
            {
                TempData["Error"] = "An error occurred while loading car details.";
                return RedirectToAction(nameof(Buy));
            }
        }

        public async Task<IActionResult> RentCar(int id)
        {
            var car = await _carsBusinessLogic.GetCarForRentalAsync(id);
            if (car == null)
                return NotFound();

            var viewModel = new CarRentalViewModel
            {
                CarId = car.Id,
                CarMake = car.Brand,
                CarModel = car.Model,
                CarYear = car.Year,
                DailyRate = car.DailyRentalPrice ?? 0,
                MinRentalDays = car.MinRentalDays,
                MaxRentalDays = car.MaxRentalDays,
                RentalTerms = car.RentalTerms,
                RentalDetails = car.RentalDetails
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentCar(CarRentalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var car = await _carsBusinessLogic.GetCarByIdAsync(model.CarId);
                if (car == null || !car.IsAvailableForRental)
                    return NotFound();

                if (model.StartDate < DateTime.Today)
                    ModelState.AddModelError(nameof(model.StartDate), "Start date cannot be in the past");

                if (model.EndDate <= model.StartDate)
                    ModelState.AddModelError(nameof(model.EndDate), "End date must be after start date");

                var totalDays = model.TotalDays;

                if (car.MinRentalDays.HasValue && totalDays < car.MinRentalDays.Value)
                    ModelState.AddModelError(nameof(model.EndDate), $"Minimum rental period is {car.MinRentalDays} days");

                if (car.MaxRentalDays.HasValue && totalDays > car.MaxRentalDays.Value)
                    ModelState.AddModelError(nameof(model.EndDate), $"Maximum rental period is {car.MaxRentalDays} days");

                var hasConflict = await _carsBusinessLogic.HasRentalConflictAsync(model.CarId, model.StartDate, model.EndDate);
                if (hasConflict)
                    ModelState.AddModelError("", "The car is not available for the selected dates");

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var rental = new CarRental
                    {
                        CarId = model.CarId,
                        RenterId = user!.Id,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        TotalDays = totalDays,
                        DailyRate = car.DailyRentalPrice ?? 0,
                        TotalAmount = model.TotalAmount,
                        SpecialRequests = model.SpecialRequests,
                        Status = RentalStatus.Pending
                    };

                    await _carsBusinessLogic.AddRentalAsync(rental);

                    TempData["Success"] = "Rental request submitted successfully! The owner will be notified.";
                    return RedirectToAction(nameof(MyRentals));
                }
            }

            var carData = await _carsBusinessLogic.GetCarByIdAsync(model.CarId);
            if (carData != null)
            {
                model.CarMake = carData.Brand;
                model.CarModel = carData.Model;
                model.CarYear = carData.Year;
                model.DailyRate = carData.DailyRentalPrice ?? 0;
                model.MinRentalDays = carData.MinRentalDays;
                model.MaxRentalDays = carData.MaxRentalDays;
                model.RentalTerms = carData.RentalTerms;
                model.RentalDetails = carData.RentalDetails;
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyListings()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var cars = await _carService.GetCarsByOwnerAsync(user!.Id);
                return View(cars);
            }
            catch
            {
                TempData["Error"] = "An error occurred while loading your car listings.";
                return View(new List<Car>());
            }
        }

        public async Task<IActionResult> MyRentals()
        {
            var user = await _userManager.GetUserAsync(User);
            var rentals = await _carsBusinessLogic.GetMyRentalsAsync(user!.Id);
            return View(rentals);
        }

        public async Task<IActionResult> RentalRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            var rentalRequests = await _carsBusinessLogic.GetRentalRequestsForOwnerAsync(user!.Id);
            return View(rentalRequests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmRental(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var success = await _carsBusinessLogic.ConfirmRentalAsync(id, currentUser!.Id);

            if (success)
            {
                TempData["Success"] = "Rental request confirmed successfully!";
            }
            else
            {
                TempData["Error"] = "Unable to confirm rental request.";
            }

            return RedirectToAction(nameof(RentalRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRental(int id)
        {
            var rental = await _carsBusinessLogic.GetRentalWithCarAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (rental == null)
                return NotFound();

            if (rental.RenterId != user!.Id && rental.Car.OwnerId != user.Id)
                return Forbid();

            try
            {
                if (rental.Status == RentalStatus.Pending || rental.Status == RentalStatus.Confirmed)
                {
                    rental.Status = RentalStatus.Cancelled;
                    await _carsBusinessLogic.SaveChangesAsync();
                    TempData["Success"] = "Rental cancelled successfully!";
                }
                else
                {
                    TempData["Error"] = "Unable to cancel rental.";
                }
            }
            catch
            {
                TempData["Error"] = "An error occurred while cancelling the rental.";
            }

            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("RentalRequests"))
                return RedirectToAction(nameof(RentalRequests));

            return RedirectToAction(nameof(MyRentals));
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                if (car == null)
                    return NotFound();

                var user = await _userManager.GetUserAsync(User);
                if (!await _carService.CanUserEditCarAsync(id, user!.Id))
                    return Forbid();

                var model = new EditCarViewModel
                {
                    Id = car.Id,
                    Brand = car.Brand,
                    Model = car.Model,
                    Year = car.Year,
                    Mileage = car.Mileage,
                    Price = car.Price,
                    Description = car.Description,
                    IsAvailableForRental = car.IsAvailableForRental,
                    DailyRentalPrice = car.DailyRentalPrice,
                    MinRentalDays = car.MinRentalDays,
                    MaxRentalDays = car.MaxRentalDays,
                    RentalTerms = car.RentalTerms,
                    RentalDetails = car.RentalDetails,
                    CurrentPhotoPath = car.PhotoPath
                };

                return View(model);
            }
            catch
            {
                TempData["Error"] = "An error occurred while loading the car for editing.";
                return RedirectToAction(nameof(MyListings));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCarViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (!await _carService.CanUserEditCarAsync(id, user!.Id))
                return Forbid();

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                await _carService.UpdateCarAsync(viewModel);
                TempData["Success"] = "Car listing updated successfully!";
                return RedirectToAction(nameof(Details), new { id = viewModel.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating the car listing.";
                return View(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var isAdmin = await _userManager.IsInRoleAsync(user!, "Admin");

                var success = await _carService.DeleteCarAsync(id, user!.Id, isAdmin);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ?
                    "Anunțul a fost șters cu succes." :
                    "Nu aveți permisiunea să ștergeți acest anunț.";
            }
            catch
            {
                TempData["ErrorMessage"] = "A apărut o eroare la ștergerea anunțului.";
            }

            return User.IsInRole("Admin") ? RedirectToAction("Buy") : RedirectToAction("MyListings");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDelete(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var success = await _carBusinessLogic.DeleteCarByAdminAsync(id, user!.Id);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ?
                    "Anunțul a fost șters cu succes." :
                    "Nu s-a putut șterge anunțul.";
            }
            catch
            {
                TempData["ErrorMessage"] = "A apărut o eroare la ștergerea anunțului.";
            }

            return RedirectToAction("Buy");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminManage()
        {
            try
            {
                var allCars = await _carService.GetAllCarsAsync();
                return View(allCars);
            }
            catch
            {
                TempData["ErrorMessage"] = "A apărut o eroare la încărcarea anunțurilor.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
