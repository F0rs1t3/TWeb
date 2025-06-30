using TWeb.DTOs;

namespace TWeb.Business.Interfaces
{
    public interface ISmartPricingBusinessLogic
    {
        Task<SmartPricingDto?> CalculateSmartPricingAsync(int carId, DateTime startDate, DateTime endDate);
    }
}