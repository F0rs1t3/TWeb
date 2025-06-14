using TWeb.Business.Interfaces;
using TWeb.DTOs;
using TWeb.Repositories.Interfaces;

namespace TWeb.Business
{
    public class SmartPricingBusinessLogic : ISmartPricingBusinessLogic
    {
        private readonly ICarRepository _carRepository;
        private readonly ICarRentalRepository _rentalRepository;

        public SmartPricingBusinessLogic(ICarRepository carRepository, ICarRentalRepository rentalRepository)
        {
            _carRepository = carRepository;
            _rentalRepository = rentalRepository;
        }

        public async Task<SmartPricingDto> CalculateSmartPricingAsync(int carId, DateTime startDate, DateTime endDate)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car?.DailyRentalPrice == null) return null;

            var basePricing = new SmartPricingDto
            {
                CarId = carId,
                BasePrice = car.DailyRentalPrice.Value,
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = (endDate - startDate).Days
            };

            // Apply demand-based pricing
            var demandMultiplier = await CalculateDemandMultiplierAsync(startDate, endDate);
            basePricing.DemandMultiplier = demandMultiplier;

            // Apply duration discount
            var durationDiscount = CalculateDurationDiscount(basePricing.TotalDays);
            basePricing.DurationDiscount = durationDiscount;

            // Apply seasonal pricing
            var seasonalMultiplier = CalculateSeasonalMultiplier(startDate, endDate);
            basePricing.SeasonalMultiplier = seasonalMultiplier;

            // Calculate final price
            var adjustedDailyRate = basePricing.BasePrice * demandMultiplier * seasonalMultiplier * (1 - durationDiscount);
            basePricing.AdjustedDailyRate = adjustedDailyRate;
            basePricing.TotalPrice = adjustedDailyRate * basePricing.TotalDays;

            basePricing.PricingFactors = GeneratePricingExplanation(demandMultiplier, durationDiscount, seasonalMultiplier);

            return basePricing;
        }

        private async Task<decimal> CalculateDemandMultiplierAsync(DateTime startDate, DateTime endDate)
        {
            // Check how many cars are booked during this period
            var totalCars = await _carRepository.GetAvailableRentalCarsCountAsync();
            var bookedCars = await _rentalRepository.GetBookedCarsCountAsync(startDate, endDate);
            
            if (totalCars == 0) return 1.0m;
            
            var occupancyRate = (decimal)bookedCars / totalCars;
            
            return occupancyRate switch
            {
                > 0.8m => 1.3m, // High demand
                > 0.6m => 1.15m, // Medium-high demand
                > 0.4m => 1.0m, // Normal demand
                > 0.2m => 0.95m, // Low demand
                _ => 0.9m // Very low demand
            };
        }

        private decimal CalculateDurationDiscount(int totalDays)
        {
            return totalDays switch
            {
                >= 30 => 0.15m, // 15% discount for monthly rentals
                >= 14 => 0.10m, // 10% discount for 2+ weeks
                >= 7 => 0.05m,  // 5% discount for weekly rentals
                _ => 0m // No discount for short rentals
            };
        }

        private decimal CalculateSeasonalMultiplier(DateTime startDate, DateTime endDate)
        {
            // Summer months (June-August) have higher demand
            var summerMonths = new[] { 6, 7, 8 };
            var holidayMonths = new[] { 12, 1 }; // December, January
            
            var avgMonth = (startDate.Month + endDate.Month) / 2.0;
            
            if (summerMonths.Contains((int)avgMonth))
                return 1.2m;
            else if (holidayMonths.Contains((int)avgMonth))
                return 1.15m;
            else
                return 1.0m;
        }

        private List<string> GeneratePricingExplanation(decimal demandMultiplier, decimal durationDiscount, decimal seasonalMultiplier)
        {
            var factors = new List<string>();

            if (demandMultiplier > 1.1m)
                factors.Add($"High demand period (+{(demandMultiplier - 1) * 100:F0}%)");
            else if (demandMultiplier < 0.95m)
                factors.Add($"Low demand period ({(demandMultiplier - 1) * 100:F0}%)");

            if (durationDiscount > 0)
                factors.Add($"Long-term rental discount (-{durationDiscount * 100:F0}%)");

            if (seasonalMultiplier > 1.05m)
                factors.Add($"Peak season surcharge (+{(seasonalMultiplier - 1) * 100:F0}%)");

            if (!factors.Any())
                factors.Add("Standard pricing applied");

            return factors;
        }
    }
}