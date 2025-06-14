namespace TWeb.DTOs
{
    public class CarPerformanceDto
    {
        public int CarId { get; set; }
        public CarDto CarDetails { get; set; } = new();
        public int TotalRentals { get; set; }
        public int CompletedRentals { get; set; }
        public double CancellationRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRentalDuration { get; set; }
        public double OccupancyRate { get; set; }
        public List<CarRentalDto> RecentRentals { get; set; } = new();
        public Dictionary<string, decimal> MonthlyRevenue { get; set; } = new();
        public List<string> PerformanceInsights { get; set; } = new();
    }
}