using System.ComponentModel.DataAnnotations;

namespace TWeb.Models
{
    public class CarRental
    {
        public int Id { get; set; }
        
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;
        
        [Required]
        public string RenterId { get; set; } = string.Empty;
        public ApplicationUser Renter { get; set; } = null!;
        
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        
        [Required]
        [Display(Name = "Total Days")]
        public int TotalDays { get; set; }
        
        [Required]
        [Display(Name = "Daily Rate")]
        public decimal DailyRate { get; set; }
        
        [Required]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Special Requests")]
        [StringLength(500)]
        public string? SpecialRequests { get; set; }
        
        public RentalStatus Status { get; set; } = RentalStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ConfirmedAt { get; set; }
        
        public string? Notes { get; set; }
    }
    
    public enum RentalStatus
    {
        Pending,
        Confirmed,
        Active,
        Completed,
        Cancelled
    }
}