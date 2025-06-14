using TWeb.DTOs;
using TWeb.Models.ViewModels;

namespace TWeb.Business.Interfaces
{
    public interface ICarBusinessLogic
    {
        Task<IEnumerable<CarDto>> GetCarsForSaleAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<IEnumerable<CarDto>> GetCarsForRentalAsync(string? search = null, string? brand = null, decimal? minPrice = null, decimal? maxPrice = null);
        Task<IEnumerable<CarDto>> GetCarsByOwnerAsync(string ownerId);
        Task<CarDto?> GetCarByIdAsync(int carId);
        Task<CarDto> CreateCarAsync(CreateCarViewModel model, string ownerId);
        Task<CarDto> UpdateCarAsync(int carId, CreateCarViewModel model, string ownerId);
        Task<bool> DeleteCarAsync(int carId, string ownerId);
        Task<IEnumerable<string>> GetBrandsAsync();
        Task<IEnumerable<string>> GetRentalBrandsAsync();
        Task<bool> IsOwnerAsync(int carId, string userId);
        Task<bool> CanDeleteCarAsync(int carId, string userId);
    }
}