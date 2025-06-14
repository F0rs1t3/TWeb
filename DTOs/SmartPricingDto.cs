namespace TWeb.DTOs
{
    public class SmartPricingDto
    {
        public int CarId { get; set; }
        public decimal BasePrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public decimal DemandMultiplier { get; set; }
        public decimal DurationDiscount { get; set; }
        public decimal SeasonalMultiplier { get; set; }
        public decimal AdjustedDailyRate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string> PricingFactors { get; set; } = new();
        public decimal PotentialSavings => (BasePrice * TotalDays) - TotalPrice;
    }
}