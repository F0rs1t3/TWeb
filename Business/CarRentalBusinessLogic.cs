using TWeb.Business.Interfaces;
using TWeb.DTOs;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Repositories.Interfaces;

namespace TWeb.Business
{
    public class CarRentalBusinessLogic : ICarRentalBusinessLogic
    {
        private readonly ICarRentalRepository _rentalRepository;
        private readonly ICarRepository _carRepository;

        public CarRentalBusinessLogic(ICarRentalRepository rentalRepository, ICarRepository carRepository)
        {
            _rentalRepository = rentalRepository;
            _carRepository = carRepository;
        }

        public async Task<CarRentalViewModel?> GetRentalViewModelAsync(int carId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null || !car.IsAvailableForRental)
            {
                return null;
            }

            return new CarRentalViewModel
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
        }

        public async Task<CarRentalDto> CreateRentalRequestAsync(CarRentalViewModel model, string renterId)
        {
            var car = await _carRepository.GetCarByIdAsync(model.CarId);
            if (car == null || !car.IsAvailableForRental)
            {
                throw new ArgumentException("Car not found or not available for rental");
            }

            await ValidateRentalRequest(model, car);

            var rental = new CarRental
            {
                CarId = model.CarId,
                RenterId = renterId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TotalDays = model.TotalDays,
                DailyRate = car.DailyRentalPrice ?? 0,
                TotalAmount = model.TotalAmount,
                SpecialRequests = model.SpecialRequests,
                Status = RentalStatus.Pending
            };

            var createdRental = await _rentalRepository.CreateRentalAsync(rental);
            var rentalWithDetails = await _rentalRepository.GetRentalByIdAsync(createdRental.Id);
            
            return MapToDto(rentalWithDetails!);
        }

        public async Task<IEnumerable<CarRentalDto>> GetRentalsByRenterAsync(string renterId)
        {
            var rentals = await _rentalRepository.GetRentalsByRenterAsync(renterId);
            return rentals.Select(MapToDto);
        }

        public async Task<IEnumerable<CarRentalDto>> GetRentalRequestsByOwnerAsync(string ownerId)
        {
            var rentals = await _rentalRepository.GetRentalsByOwnerAsync(ownerId);
            return rentals.Select(MapToDto);
        }

        public async Task<bool> ConfirmRentalAsync(int rentalId, string ownerId)
        {
            var rental = await _rentalRepository.GetRentalByIdAsync(rentalId);
            if (rental == null || rental.Car.OwnerId != ownerId || rental.Status != RentalStatus.Pending)
            {
                return false;
            }

            rental.Status = RentalStatus.Confirmed;
            rental.ConfirmedAt = DateTime.UtcNow;
            await _rentalRepository.UpdateRentalAsync(rental);
            
            return true;
        }

        public async Task<bool> CancelRentalAsync(int rentalId, string userId)
        {
            var rental = await _rentalRepository.GetRentalByIdAsync(rentalId);
            if (rental == null)
            {
                return false;
            }

            // Allow cancellation by either the renter or the car owner
            if (rental.RenterId != userId && rental.Car.OwnerId != userId)
            {
                return false;
            }

            if (rental.Status == RentalStatus.Pending || rental.Status == RentalStatus.Confirmed)
            {
                rental.Status = RentalStatus.Cancelled;
                await _rentalRepository.UpdateRentalAsync(rental);
                return true;
            }

            return false;
        }

        public async Task<bool> ValidateRentalDatesAsync(int carId, DateTime startDate, DateTime endDate, int? excludeRentalId = null)
        {
            if (startDate < DateTime.Today)
            {
                throw new ArgumentException("Start date cannot be in the past");
            }

            if (endDate <= startDate)
            {
                throw new ArgumentException("End date must be after start date");
            }

            var hasConflict = await _rentalRepository.HasConflictingRentalsAsync(carId, startDate, endDate, excludeRentalId);
            if (hasConflict)
            {
                throw new ArgumentException("The car is not available for the selected dates");
            }

            return true;
        }

        public async Task<decimal> CalculateRentalCostAsync(int carId, DateTime startDate, DateTime endDate)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null || !car.DailyRentalPrice.HasValue)
            {
                return 0;
            }

            var totalDays = (endDate - startDate).Days;
            return totalDays * car.DailyRentalPrice.Value;
        }

        private async Task ValidateRentalRequest(CarRentalViewModel model, Car car)
        {
            await ValidateRentalDatesAsync(model.CarId, model.StartDate, model.EndDate);

            var totalDays = model.TotalDays;
            if (car.MinRentalDays.HasValue && totalDays < car.MinRentalDays.Value)
            {
                throw new ArgumentException($"Minimum rental period is {car.MinRentalDays} days");
            }

            if (car.MaxRentalDays.HasValue && totalDays > car.MaxRentalDays.Value)
            {
                throw new ArgumentException($"Maximum rental period is {car.MaxRentalDays} days");
            }
        }

        private CarRentalDto MapToDto(CarRental rental)
        {
            return new CarRentalDto
            {
                Id = rental.Id,
                CarId = rental.CarId,
                RenterId = rental.RenterId,
                RenterName = $"{rental.Renter?.FirstName} {rental.Renter?.LastName}".Trim(),
                RenterEmail = rental.Renter?.Email ?? string.Empty,
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                TotalDays = rental.TotalDays,
                DailyRate = rental.DailyRate,
                TotalAmount = rental.TotalAmount,
                SpecialRequests = rental.SpecialRequests,
                Notes = rental.Notes,
                Status = rental.Status,
                CreatedAt = rental.CreatedAt,
                ConfirmedAt = rental.ConfirmedAt,
                Car = new CarDto
                {
                    Id = rental.Car.Id,
                    Brand = rental.Car.Brand,
                    Model = rental.Car.Model,
                    Year = rental.Car.Year,
                    PhotoPath = rental.Car.PhotoPath,
                    OwnerName = $"{rental.Car.Owner?.FirstName} {rental.Car.Owner?.LastName}".Trim()
                }
            };
        }
    }
}