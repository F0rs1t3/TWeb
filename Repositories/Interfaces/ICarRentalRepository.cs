using TWeb.Models;

namespace TWeb.Repositories.Interfaces
{
    public interface ICarRentalRepository
    {
        Task<CarRental> CreateRentalAsync(CarRental rental);
        Task<CarRental> UpdateRentalAsync(CarRental rental);
        Task<CarRental?> GetRentalByIdAsync(int id);
        Task<IEnumerable<CarRental>> GetRentalsByRenterAsync(string renterId);
        Task<IEnumerable<CarRental>> GetRentalsByOwnerAsync(string ownerId);
        Task<bool> HasConflictingRentalsAsync(int carId, DateTime startDate, DateTime endDate, int? excludeRentalId = null);
        Task<bool> HasActiveRentalsAsync(int carId);
        Task<int> GetBookedCarsCountAsync();
        Task<int> GetBookedCarsCountAsync(DateTime startDate, DateTime endDate);
    }
}
