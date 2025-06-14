using TWeb.Business.Interfaces;
using TWeb.DTOs;
using TWeb.Models.ViewModels;
using TWeb.Services.Interfaces;

namespace TWeb.Services
{
    public class CarRentalService : ICarRentalService
    {
        private readonly ICarRentalBusinessLogic _rentalBusinessLogic;
        private readonly ILogger<CarRentalService> _logger;

        public CarRentalService(ICarRentalBusinessLogic rentalBusinessLogic, ILogger<CarRentalService> logger)
        {
            _rentalBusinessLogic = rentalBusinessLogic;
            _logger = logger;
        }

        public async Task<CarRentalViewModel?> GetRentalFormAsync(int carId)
        {
            try
            {
                _logger.LogInformation("Getting rental form for car: {CarId}", carId);
                return await _rentalBusinessLogic.GetRentalViewModelAsync(carId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rental form for car: {CarId}", carId);
                throw;
            }
        }

        public async Task<CarRentalDto> SubmitRentalRequestAsync(CarRentalViewModel model, string userId)
        {
            try
            {
                _logger.LogInformation("Submitting rental request for car {CarId} by user: {UserId}", model.CarId, userId);
                return await _rentalBusinessLogic.CreateRentalRequestAsync(model, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting rental request for car {CarId} by user: {UserId}", model.CarId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<CarRentalDto>> GetUserRentalsAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Getting rentals for user: {UserId}", userId);
                return await _rentalBusinessLogic.GetRentalsByRenterAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rentals for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<CarRentalDto>> GetOwnerRentalRequestsAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Getting rental requests for owner: {UserId}", userId);
                return await _rentalBusinessLogic.GetRentalRequestsByOwnerAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rental requests for owner: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ApproveRentalRequestAsync(int rentalId, string userId)
        {
            try
            {
                _logger.LogInformation("Approving rental request {RentalId} by user: {UserId}", rentalId, userId);
                return await _rentalBusinessLogic.ConfirmRentalAsync(rentalId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving rental request {RentalId} by user: {UserId}", rentalId, userId);
                throw;
            }
        }

        public async Task<bool> CancelRentalAsync(int rentalId, string userId)
        {
            try
            {
                _logger.LogInformation("Cancelling rental {RentalId} by user: {UserId}", rentalId, userId);
                return await _rentalBusinessLogic.CancelRentalAsync(rentalId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling rental {RentalId} by user: {UserId}", rentalId, userId);
                throw;
            }
        }

        public async Task<decimal> CalculateRentalCostAsync(int carId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _rentalBusinessLogic.CalculateRentalCostAsync(carId, startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating rental cost for car {CarId}", carId);
                throw;
            }
        }
    }
}