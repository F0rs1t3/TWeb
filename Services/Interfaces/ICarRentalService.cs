using TWeb.DTOs;
using TWeb.Models.ViewModels;

namespace TWeb.Services.Interfaces
{
    public interface ICarRentalService
    {
        Task<CarRentalViewModel?> GetRentalFormAsync(int carId);
        Task<CarRentalDto> SubmitRentalRequestAsync(CarRentalViewModel model, string userId);
        Task<IEnumerable<CarRentalDto>> GetUserRentalsAsync(string userId);
        Task<IEnumerable<CarRentalDto>> GetOwnerRentalRequestsAsync(string userId);
        Task<bool> ApproveRentalRequestAsync(int rentalId, string userId);
        Task<bool> CancelRentalAsync(int rentalId, string userId);
        Task<decimal> CalculateRentalCostAsync(int carId, DateTime startDate, DateTime endDate);
    }
}