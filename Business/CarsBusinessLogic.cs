using Microsoft.EntityFrameworkCore;
using TWeb.Business.Interfaces;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Data;

namespace TWeb.Business
{
    public class CarsBusinessLogic : ICarsBusinessLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationBusinessLogic _notificationBusinessLogic;

        public CarsBusinessLogic(ApplicationDbContext context, INotificationBusinessLogic notificationBusinessLogic)
        {
            _context = context;
            _notificationBusinessLogic = notificationBusinessLogic;
        }

        public async Task<Car?> GetCarForRentalAsync(int carId)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.Id == carId && c.IsAvailableForRental);
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars.FindAsync(id);
        }

        public async Task<bool> HasRentalConflictAsync(int carId, DateTime start, DateTime end)
        {
            return await _context.CarRentals
                .AnyAsync(r => r.CarId == carId &&
                    r.Status != RentalStatus.Cancelled &&
                    ((r.StartDate <= start && r.EndDate >= start) ||
                     (r.StartDate <= end && r.EndDate >= end) ||
                     (r.StartDate >= start && r.EndDate <= end)));
        }

        public async Task AddRentalAsync(CarRental rental)
        {
            _context.CarRentals.Add(rental);
            await _context.SaveChangesAsync();

            // Get the car information for the notification
            var car = await _context.Cars.FindAsync(rental.CarId);
            if (car != null)
            {
                // Send notification to car owner about new rental request
                await _notificationBusinessLogic.CreateRentalRequestNotificationAsync(rental.Id, car.OwnerId);
            }
        }

        public async Task<List<CarRental>> GetMyRentalsAsync(string userId)
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .Include(r => r.Car.Owner)
                .Where(r => r.RenterId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CarRental>> GetRentalRequestsForOwnerAsync(string ownerId)
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .Include(r => r.Renter)
                .Where(r => r.Car.OwnerId == ownerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<CarRental?> GetRentalWithCarAsync(int rentalId)
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentalId);
        }

        public async Task<bool> ConfirmRentalAsync(int rentalId, string ownerId)
        {
            var rental = await _context.CarRentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == rentalId);

            if (rental == null || rental.Car.OwnerId != ownerId || rental.Status != RentalStatus.Pending)
            {
                return false;
            }

            rental.Status = RentalStatus.Confirmed;
            rental.ConfirmedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send notification to renter about rental confirmation
            await _notificationBusinessLogic.CreateRentalConfirmationNotificationAsync(rentalId, rental.RenterId);

            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}