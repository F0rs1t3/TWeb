using TWeb.DTOs;
using TWeb.Models.ViewModels;

namespace TWeb.Business.Interfaces
{
    public interface ICarRentalBusinessLogic
    {
        Task<CarRentalViewModel?> GetRentalViewModelAsync(int carId);
        Task<CarRentalDto> CreateRentalRequestAsync(CarRentalViewModel model, string renterId);
        Task<IEnumerable<CarRentalDto>> GetRentalsByRenterAsync(string renterId);
        Task<IEnumerable<CarRentalDto>> GetRentalRequestsByOwnerAsync(string ownerId);
        Task<bool> ConfirmRentalAsync(int rentalId, string ownerId);
        Task<bool> CancelRentalAsync(int rentalId, string userId);
        Task<bool> ValidateRentalDatesAsync(int carId, DateTime startDate, DateTime endDate, int? excludeRentalId = null);
        Task<decimal> CalculateRentalCostAsync(int carId, DateTime startDate, DateTime endDate);
    }
}