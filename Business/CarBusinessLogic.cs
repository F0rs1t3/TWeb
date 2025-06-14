using Microsoft.EntityFrameworkCore;
using TWeb.Business.Interfaces;
using TWeb.Data;
using TWeb.DTOs;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Repositories.Interfaces;

namespace TWeb.Business
{
    public class CarBusinessLogic : ICarBusinessLogic
    {
        private readonly ICarRepository _carRepository;
        private readonly ICarRentalRepository _rentalRepository;
        private readonly ILogger<CarBusinessLogic> _logger;

        public CarBusinessLogic(
            ICarRepository carRepository, 
            ICarRentalRepository rentalRepository,
            ILogger<CarBusinessLogic> logger)
        {
            _carRepository = carRepository;
            _rentalRepository = rentalRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CarDto>> GetCarsForSaleAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var cars = await _carRepository.GetCarsForSaleAsync(search, brand, minPrice, maxPrice);
            return cars.Select(MapToDto);
        }

        public async Task<IEnumerable<CarDto>> GetCarsForRentalAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var cars = await _carRepository.GetCarsForRentalAsync(search, brand, minPrice, maxPrice);
            return cars.Select(MapToDto);
        }

        public async Task<IEnumerable<CarDto>> GetCarsByOwnerAsync(string ownerId)
        {
            var cars = await _carRepository.GetCarsByOwnerAsync(ownerId);
            return cars.Select(MapToDto);
        }

        public async Task<CarDto?> GetCarByIdAsync(int carId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            return car != null ? MapToDto(car) : null;
        }

        public async Task<CarDto> CreateCarAsync(CreateCarViewModel model, string ownerId)
        {
            ValidateCarModel(model);

            var car = new Car
            {
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                Mileage = model.Mileage,
                Price = model.Price,
                Description = model.Description,
                OwnerId = ownerId,
                IsAvailableForRental = model.IsAvailableForRental,
                DailyRentalPrice = model.DailyRentalPrice,
                MinRentalDays = model.MinRentalDays,
                MaxRentalDays = model.MaxRentalDays,
                RentalTerms = model.RentalTerms,
                RentalDetails = model.RentalDetails,
                PhotoPath = model.Photo?.FileName // Simplified for now
            };

            var createdCar = await _carRepository.CreateCarAsync(car);
            return MapToDto(createdCar);
        }

        public async Task<CarDto> UpdateCarAsync(int carId, CreateCarViewModel model, string ownerId)
        {
            var existingCar = await _carRepository.GetCarByIdAsync(carId);
            if (existingCar == null || existingCar.OwnerId != ownerId)
            {
                throw new UnauthorizedAccessException("Car not found or access denied");
            }

            ValidateCarModel(model);

            existingCar.Brand = model.Brand;
            existingCar.Model = model.Model;
            existingCar.Year = model.Year;
            existingCar.Mileage = model.Mileage;
            existingCar.Price = model.Price;
            existingCar.Description = model.Description;
            existingCar.IsAvailableForRental = model.IsAvailableForRental;
            existingCar.DailyRentalPrice = model.DailyRentalPrice;
            existingCar.MinRentalDays = model.MinRentalDays;
            existingCar.MaxRentalDays = model.MaxRentalDays;
            existingCar.RentalTerms = model.RentalTerms;
            existingCar.RentalDetails = model.RentalDetails;

            if (model.Photo != null)
            {
                existingCar.PhotoPath = model.Photo.FileName;
            }

            var updatedCar = await _carRepository.UpdateCarAsync(existingCar);
            return MapToDto(updatedCar);
        }

        public async Task<bool> DeleteCarAsync(int carId, string ownerId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null || car.OwnerId != ownerId)
            {
                return false;
            }

            if (await _carRepository.HasActiveRentalsAsync(carId))
            {
                throw new InvalidOperationException("Cannot delete car with active rental bookings");
            }

            return await _carRepository.DeleteCarAsync(carId);
        }

        public async Task<IEnumerable<string>> GetBrandsAsync()
        {
            return await _carRepository.GetDistinctBrandsAsync();
        }

        public async Task<IEnumerable<string>> GetRentalBrandsAsync()
        {
            return await _carRepository.GetDistinctRentalBrandsAsync();
        }

        public async Task<bool> IsOwnerAsync(int carId, string userId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            return car != null && car.OwnerId == userId;
        }

        public async Task<bool> CanDeleteCarAsync(int carId, string userId)
        {
            if (!await IsOwnerAsync(carId, userId))
                return false;

            var hasActiveRentals = await _rentalRepository.HasActiveRentalsAsync(carId);
            return !hasActiveRentals;
        }

        private void ValidateCarModel(CreateCarViewModel model)
        {
            if (model.IsAvailableForRental)
            {
                if (!model.DailyRentalPrice.HasValue || model.DailyRentalPrice <= 0)
                {
                    throw new ArgumentException("Daily rental price is required when car is available for rental");
                }

                if (model.MinRentalDays.HasValue && model.MaxRentalDays.HasValue && 
                    model.MinRentalDays > model.MaxRentalDays)
                {
                    throw new ArgumentException("Maximum rental days must be greater than or equal to minimum rental days");
                }
            }
        }

        private CarDto MapToDto(Car car)
        {
            return new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Mileage = car.Mileage,
                Price = car.Price,
                Description = car.Description,
                PhotoPath = car.PhotoPath,
                OwnerId = car.OwnerId,
                OwnerName = $"{car.Owner?.FirstName} {car.Owner?.LastName}".Trim(),
                IsForSale = car.IsForSale,
                IsAvailableForRental = car.IsAvailableForRental,
                DailyRentalPrice = car.DailyRentalPrice,
                SellingPrice = car.SellingPrice,
                SellingDescription = car.SellingDescription,
                MinRentalDays = car.MinRentalDays,
                MaxRentalDays = car.MaxRentalDays,
                RentalTerms = car.RentalTerms,
                RentalDetails = car.RentalDetails,
                CreatedAt = car.CreatedAt
            };
        }
    }
}
