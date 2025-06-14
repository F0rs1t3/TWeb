using Microsoft.EntityFrameworkCore;
using TWeb.Data;
using TWeb.Models;
using TWeb.Repositories.Interfaces;

namespace TWeb.Repositories
{
    public class CarRentalRepository : ICarRentalRepository
    {
        private readonly ApplicationDbContext _context;

        public CarRentalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CarRental> CreateRentalAsync(CarRental rental)
        {
            _context.CarRentals.Add(rental);
            await _context.SaveChangesAsync();
            return rental;
        }

        public async Task<CarRental> UpdateRentalAsync(CarRental rental)
        {
            _context.CarRentals.Update(rental);
            await _context.SaveChangesAsync();
            return rental;
        }

        public async Task<CarRental?> GetRentalByIdAsync(int id)
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .Include(r => r.Car.Owner)
                .Include(r => r.Renter)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<CarRental>> GetRentalsByRenterAsync(string renterId)
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .Include(r => r.Car.Owner)
                .Include(r => r.Renter)
                .Where(r => r.RenterId == renterId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CarRental>> GetRentalsByOwnerAsync(string ownerId)
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .Include(r => r.Car.Owner)
                .Include(r => r.Renter)
                .Where(r => r.Car.OwnerId == ownerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingRentalsAsync(int carId, DateTime startDate, DateTime endDate, int? excludeRentalId = null)
        {
            var query = _context.CarRentals
                .Where(r => r.CarId == carId && 
                           r.Status != RentalStatus.Cancelled &&
                           ((r.StartDate <= startDate && r.EndDate >= startDate) ||
                            (r.StartDate <= endDate && r.EndDate >= endDate) ||
                            (r.StartDate >= startDate && r.EndDate <= endDate)));

            if (excludeRentalId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRentalId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveRentalsAsync(int carId)
        {
            return await _context.CarRentals
                .AnyAsync(r => r.CarId == carId && 
                              (r.Status == RentalStatus.Confirmed || r.Status == RentalStatus.Active));
        }

        public async Task<int> GetBookedCarsCountAsync()
        {
            return await _context.CarRentals
                .Where(r => r.Status == RentalStatus.Confirmed || r.Status == RentalStatus.Active)
                .Select(r => r.CarId)
                .Distinct()
                .CountAsync();
        }

        public async Task<int> GetBookedCarsCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.CarRentals
                .Where(r => (r.Status == RentalStatus.Confirmed || r.Status == RentalStatus.Active) &&
                           ((r.StartDate <= startDate && r.EndDate >= startDate) ||
                            (r.StartDate <= endDate && r.EndDate >= endDate) ||
                            (r.StartDate >= startDate && r.EndDate <= endDate)))
                .Select(r => r.CarId)
                .Distinct()
                .CountAsync();
        }
    }
}