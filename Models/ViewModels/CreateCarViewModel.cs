using System.ComponentModel.DataAnnotations;

namespace TWeb.Models.ViewModels
{
        public class CreateCarViewModel
        {
            [Required(ErrorMessage = "Brand is required")]
            [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
            public string Brand { get; set; } = string.Empty;
        
            [Required(ErrorMessage = "Model is required")]
            [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
            public string Model { get; set; } = string.Empty;
        
            [Required(ErrorMessage = "Year is required")]
            [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
            public int Year { get; set; }
        
            [Required(ErrorMessage = "Mileage is required")]
            [Range(0, int.MaxValue, ErrorMessage = "Mileage must be a positive number")]
            public int Mileage { get; set; }
        
            [Required(ErrorMessage = "Base price is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
            [DataType(DataType.Currency)]
            [Display(Name = "Base Price")]
            public decimal Price { get; set; }
        
            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
            public string? Description { get; set; }
        
            // Selling options
            [Display(Name = "Available for Sale")]
            public bool IsForSale { get; set; } = true;
        
            [Display(Name = "Selling Price")]
            [DataType(DataType.Currency)]
            public decimal? SellingPrice { get; set; }
        
            [Display(Name = "Selling Description")]
            [StringLength(1000, ErrorMessage = "Selling description cannot exceed 1000 characters")]
            public string? SellingDescription { get; set; }
        
            // Rental-specific properties
            [Display(Name = "Available for Rental")]
            public bool IsAvailableForRental { get; set; } = false;
        
            [Display(Name = "Daily Rental Price")]
            [DataType(DataType.Currency)]
            public decimal? DailyRentalPrice { get; set; }
        
            [Display(Name = "Minimum Rental Days")]
            [Range(1, 365, ErrorMessage = "Minimum rental days must be between 1 and 365")]
            public int? MinRentalDays { get; set; }
        
            [Display(Name = "Maximum Rental Days")]
            [Range(1, 365, ErrorMessage = "Maximum rental days must be between 1 and 365")]
            public int? MaxRentalDays { get; set; }
        
            [Display(Name = "Rental Terms")]
            [StringLength(500, ErrorMessage = "Rental terms cannot exceed 500 characters")]
            public string? RentalTerms { get; set; }
        
            [Display(Name = "Rental Details")]
            [StringLength(1000, ErrorMessage = "Rental details cannot exceed 1000 characters")]
            public string? RentalDetails { get; set; }
        
            [Display(Name = "Car Photo")]
            public IFormFile? Photo { get; set; }
        }
}