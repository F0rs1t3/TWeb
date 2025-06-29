using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Business.Interfaces
{
    public interface ICarsBusinessLogic
    {
        Task<Car?> GetCarForRentalAsync(int carId);
        Task<Car?> GetCarByIdAsync(int id);
        Task<bool> HasRentalConflictAsync(int carId, DateTime startDate, DateTime endDate);
        Task AddRentalAsync(CarRental rental);
        Task<List<CarRental>> GetMyRentalsAsync(string userId);
        Task<List<CarRental>> GetRentalRequestsForOwnerAsync(string ownerId);
        Task<CarRental?> GetRentalWithCarAsync(int rentalId);
        Task<bool> ConfirmRentalAsync(int rentalId, string ownerId);
        Task SaveChangesAsync();
    }
}
