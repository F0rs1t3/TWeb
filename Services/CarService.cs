using Microsoft.EntityFrameworkCore;
using TWeb.Data;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Services.Interfaces;

namespace TWeb.Services
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarService> _logger;
        private readonly IFileUploadService _fileUploadService;

        public CarService(ApplicationDbContext context, ILogger<CarService> logger, IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all cars for admin");
                return await _context.Cars
                    .Include(c => c.Owner)
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all cars");
                throw;
            }
        }

        public async Task<IEnumerable<Car>> GetCarsForSaleAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            try
            {
                _logger.LogInformation("Fetching cars for sale with filters: search={Search}, brand={Brand}, minPrice={MinPrice}, maxPrice={MaxPrice}", 
                    search, brand, minPrice, maxPrice);
                
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cars for sale");
                throw;
            }
        }

        public async Task<IEnumerable<Car>> GetCarsForRentalAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            try
            {
                _logger.LogInformation("Fetching cars for rental with filters: search={Search}, brand={Brand}, minPrice={MinPrice}, maxPrice={MaxPrice}", 
                    search, brand, minPrice, maxPrice);
                
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cars for rental");
                throw;
            }
        }

        public async Task<IEnumerable<Car>> GetCarsByOwnerAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Fetching cars for user: {UserId}", userId);
                return await _context.Cars
                    .Include(c => c.Owner)
                    .Where(c => c.OwnerId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user cars for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching car details for ID: {CarId}", id);
                return await _context.Cars
                    .Include(c => c.Owner)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching car details for ID: {CarId}", id);
                throw;
            }
        }

        public async Task<Car> CreateCarAsync(CreateCarViewModel model, string userId)
        {
            try
            {
                _logger.LogInformation("Creating new car for user: {UserId}", userId);
                
                // Validate rental-specific fields if rental is enabled
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

                var car = new Car
                {
                    Brand = model.Brand,
                    Model = model.Model,
                    Year = model.Year,
                    Mileage = model.Mileage,
                    Price = model.Price,
                    Description = model.Description,
                    OwnerId = userId,
                    IsAvailableForRental = model.IsAvailableForRental,
                    DailyRentalPrice = model.DailyRentalPrice,
                    MinRentalDays = model.MinRentalDays,
                    MaxRentalDays = model.MaxRentalDays,
                    RentalTerms = model.RentalTerms,
                    RentalDetails = model.RentalDetails
                };

                // Handle photo upload if provided
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    if (_fileUploadService.IsValidImageFile(model.Photo))
                    {
                        car.PhotoPath = await _fileUploadService.UploadFileAsync(model.Photo, "cars");
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid image file format or size.");
                    }
                }

                _context.Cars.Add(car);
                await _context.SaveChangesAsync();

                return car;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating car for user: {UserId}", userId);
                throw;
            }
        }

        public async Task UpdateCarAsync(EditCarViewModel model)
        {
            try
            {
                _logger.LogInformation("Updating car {CarId}", model.Id);
                
                var car = await _context.Cars.FindAsync(model.Id);
                if (car == null)
                {
                    throw new ArgumentException("Car not found");
                }

                // Validate rental-specific fields if rental is enabled
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

                car.Brand = model.Brand;
                car.Model = model.Model;
                car.Year = model.Year;
                car.Mileage = model.Mileage;
                car.Price = model.Price;
                car.Description = model.Description;
                car.IsAvailableForRental = model.IsAvailableForRental;
                car.DailyRentalPrice = model.DailyRentalPrice;
                car.MinRentalDays = model.MinRentalDays;
                car.MaxRentalDays = model.MaxRentalDays;
                car.RentalTerms = model.RentalTerms;
                car.RentalDetails = model.RentalDetails;

                // Handle photo upload if provided
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    if (_fileUploadService.IsValidImageFile(model.Photo))
                    {
                        // Delete old photo if exists
                        if (!string.IsNullOrEmpty(car.PhotoPath))
                        {
                            await _fileUploadService.DeleteFileAsync(car.PhotoPath);
                        }
                        
                        car.PhotoPath = await _fileUploadService.UploadFileAsync(model.Photo, "cars");
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid image file format or size.");
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating car {CarId}", model.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCarAsync(int carId, string userId, bool isAdmin = false)
        {
            try
            {
                var canDelete = await CanUserDeleteCarAsync(carId, userId, isAdmin);
                if (!canDelete)
                {
                    return false;
                }

                var car = await _context.Cars.FindAsync(carId);
                if (car == null)
                {
                    return false;
                }

                // Șterge imaginea dacă există
                if (!string.IsNullOrEmpty(car.PhotoPath) && _fileUploadService != null)
                {
                    try
                    {
                        await _fileUploadService.DeleteFileAsync(car.PhotoPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not delete file: {PhotoPath}", car.PhotoPath);
                    }
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Car {CarId} deleted by user {UserId} (Admin: {IsAdmin})", carId, userId, isAdmin);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car: {CarId} by user: {UserId}", carId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAvailableBrandsAsync()
        {
            try
            {
                return await _context.Cars
                    .Where(c => c.IsForSale)
                    .Select(c => c.Brand)
                    .Distinct()
                    .OrderBy(b => b)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available brands");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetRentalBrandsAsync()
        {
            try
            {
                return await _context.Cars
                    .Where(c => c.IsAvailableForRental)
                    .Select(c => c.Brand)
                    .Distinct()
                    .OrderBy(b => b)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rental brands");
                throw;
            }
        }

        public async Task<bool> CanUserEditCarAsync(int carId, string userId)
        {
            try
            {
                var car = await _context.Cars.FindAsync(carId);
                return car != null && car.OwnerId == userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking edit permissions for car {CarId} and user: {UserId}", carId, userId);
                return false;
            }
        }

        public async Task<bool> CanUserDeleteCarAsync(int carId, string userId, bool isAdmin = false)
        {
            try
            {
                // Admin poate șterge orice anunț
                if (isAdmin)
                {
                    return true;
                }
        
                // Pentru utilizatori normali, verifică dacă sunt proprietarii
                var car = await _context.Cars.FindAsync(carId);
                return car != null && car.OwnerId == userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking delete permissions for car: {CarId}, user: {UserId}", carId, userId);
                throw;
            }
        }
    }
}
