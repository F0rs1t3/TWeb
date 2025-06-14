using System.ComponentModel.DataAnnotations;

namespace TWeb.Models
{
    public class Notification
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
        
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public string? RelatedEntityId { get; set; }
        public string? ActionUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum NotificationType
    {
        General,
        NewMessage,
        NewChatRequest,
        RentalRequest,
        RentalConfirmed,
        RentalCancelled,
        RentalReminder,
        MaintenanceReminder,
        CarSold,
        SystemNotification
    }
}