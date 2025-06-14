namespace TWeb.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Mileage { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? PhotoPath { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public bool IsForSale { get; set; }
        public bool IsAvailableForRental { get; set; }
        public decimal? DailyRentalPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? SellingDescription { get; set; }
        public int? MinRentalDays { get; set; }
        public int? MaxRentalDays { get; set; }
        public string? RentalTerms { get; set; }
        public string? RentalDetails { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}