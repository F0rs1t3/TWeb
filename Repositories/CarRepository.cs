using Microsoft.EntityFrameworkCore;
using TWeb.Data;
using TWeb.Models;
using TWeb.Repositories.Interfaces;

namespace TWeb.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _context;

        public CarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsForSaleAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Cars
                .Include(c => c.Owner)
                .Where(c => c.IsForSale)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Brand.Contains(search) || c.Model.Contains(search));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(c => c.Brand == brand);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price <= maxPrice.Value);
            }

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsForRentalAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Cars
                .Include(c => c.Owner)
                .Where(c => c.IsAvailableForRental)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Brand.Contains(search) || c.Model.Contains(search));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(c => c.Brand == brand);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(c => c.DailyRentalPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.DailyRentalPrice <= maxPrice.Value);
            }

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsByOwnerAsync(string userId)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .Where(c => c.OwnerId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Car> CreateCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<Car> UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<bool> DeleteCarAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
                return false;

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetDistinctBrandsAsync()
        {
            return await _context.Cars
                .Where(c => c.IsForSale)
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctRentalBrandsAsync()
        {
            return await _context.Cars
                .Where(c => c.IsAvailableForRental)
                .Select(c => c.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();
        }

        public async Task<bool> HasActiveRentalsAsync(int carId)
        {
            return await _context.CarRentals
                .AnyAsync(r => r.CarId == carId && 
                              (r.Status == RentalStatus.Confirmed || r.Status == RentalStatus.Active));
        }

        public async Task<int> GetAvailableRentalCarsCountAsync()
        {
            return await _context.Cars
                .Where(c => c.IsAvailableForRental)
                .CountAsync();
        }

        public async Task<IEnumerable<Car>> GetCarsNeedingMaintenanceAsync()
        {
            // This is a placeholder - you can implement your own logic for determining maintenance needs
            return await _context.Cars
                .Include(c => c.Owner)
                .Where(c => c.Mileage > 100000) // Example: cars with high mileage
                .ToListAsync();
        }
    }
}
