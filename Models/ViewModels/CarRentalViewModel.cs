using System.ComponentModel.DataAnnotations;

namespace TWeb.Models.ViewModels
{
    public class CarRentalViewModel
    {
        public int CarId { get; set; }
        public string CarMake { get; set; } = string.Empty;  // This can stay as CarMake for display
        public string CarModel { get; set; } = string.Empty;
        public int CarYear { get; set; }
        public decimal DailyRate { get; set; }
        public int? MinRentalDays { get; set; }
        public int? MaxRentalDays { get; set; }
        public string? RentalTerms { get; set; }
        public string? RentalDetails { get; set; }
        
        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(1);
        
        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(2);
        
        [Display(Name = "Special Requests")]
        [StringLength(500)]
        public string? SpecialRequests { get; set; }
        
        public int TotalDays => (EndDate - StartDate).Days + 1;
        public decimal TotalAmount => TotalDays * DailyRate;
    }
}