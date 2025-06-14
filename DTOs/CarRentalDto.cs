using TWeb.Models;

namespace TWeb.DTOs
{
    public class CarRentalDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string RenterId { get; set; } = string.Empty;
        public string RenterName { get; set; } = string.Empty;
        public string RenterEmail { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? SpecialRequests { get; set; }
        public string? Notes { get; set; }
        public RentalStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public CarDto Car { get; set; } = null!;
    }
}
