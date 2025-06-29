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

        public CarsBusinessLogic(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
