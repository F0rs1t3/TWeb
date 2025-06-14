namespace TWeb.DTOs
{
    public class CarRecommendationDto
    {
        public List<CarDto> RecommendedCars { get; set; } = new();
        public string ReasonForRecommendation { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
    }
}