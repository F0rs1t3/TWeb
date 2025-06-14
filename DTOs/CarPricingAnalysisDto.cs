namespace TWeb.DTOs
{
    public class CarPricingAnalysisDto
    {
        public int CarId { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal AverageMarketPrice { get; set; }
        public PriceCompetitiveness PriceCompetitiveness { get; set; }
        public decimal CurrentRentalPrice { get; set; }
        public decimal AverageRentalPrice { get; set; }
        public PriceCompetitiveness RentalPriceCompetitiveness { get; set; }
        public int SimilarCarsCount { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }

    public enum PriceCompetitiveness
    {
        BelowMarket,
        Average,
        AboveMarket,
        NotApplicable
    }
}