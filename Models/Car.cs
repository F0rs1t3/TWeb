using System.ComponentModel.DataAnnotations;

namespace TWeb.Models
{
    public class Car
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;
        
        [Required]
        public int Year { get; set; }
        
        [Required]
        public int Mileage { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public string? Description { get; set; }
        
        public string? PhotoPath { get; set; }
        
        [Required]
        public string OwnerId { get; set; } = string.Empty;
        public ApplicationUser Owner { get; set; } = null!;
        
        public bool IsForSale { get; set; } = true;
        public bool IsAvailableForRental { get; set; } = false;
        public decimal? DailyRentalPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? SellingDescription { get; set; }
        public int? MinRentalDays { get; set; }
        public int? MaxRentalDays { get; set; }
        public string? RentalTerms { get; set; }
        public string? RentalDetails { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}