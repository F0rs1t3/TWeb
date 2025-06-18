using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Services.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetCarsForSaleAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<IEnumerable<Car>> GetCarsForRentalAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<IEnumerable<string>> GetAvailableBrandsAsync();
        Task<IEnumerable<string>> GetRentalBrandsAsync();
        Task<Car?> GetCarByIdAsync(int id);
        Task<Car> CreateCarAsync(CreateCarViewModel model, string userId);
        Task UpdateCarAsync(EditCarViewModel model);
        Task<bool> CanUserEditCarAsync(int carId, string userId);
        Task<IEnumerable<Car>> GetCarsByOwnerAsync(string userId);
        Task<bool> CanUserDeleteCarAsync(int carId, string userId, bool isAdmin = false);
        Task<bool> DeleteCarAsync(int carId, string userId, bool isAdmin = false);
        Task<IEnumerable<Car>> GetAllCarsAsync();
    }
}
