using Microsoft.EntityFrameworkCore;
using TWeb.Data;
using TWeb.Models;

namespace BusinessLogic
{
    public class CarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Car>> GetAllCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Car>> GetCarsByOwnerAsync(string ownerId)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .Where(c => c.OwnerId == ownerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Car> AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<bool> UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> IsOwnerAsync(int carId, string userId)
        {
            var car = await _context.Cars.FindAsync(carId);
            return car?.OwnerId == userId;
        }
    }
}