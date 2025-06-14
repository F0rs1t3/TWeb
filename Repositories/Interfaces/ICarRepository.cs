using TWeb.Models;

namespace TWeb.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<IEnumerable<Car>> GetCarsForSaleAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<IEnumerable<Car>> GetCarsForRentalAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<IEnumerable<Car>> GetCarsByOwnerAsync(string userId);
        Task<Car?> GetCarByIdAsync(int id);
        Task<Car> CreateCarAsync(Car car);
        Task<Car> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int carId);
        Task<IEnumerable<string>> GetDistinctBrandsAsync();
        Task<IEnumerable<string>> GetDistinctRentalBrandsAsync();
        Task<bool> HasActiveRentalsAsync(int carId);
        Task<int> GetAvailableRentalCarsCountAsync();
        Task<IEnumerable<Car>> GetCarsNeedingMaintenanceAsync();
    }
}
