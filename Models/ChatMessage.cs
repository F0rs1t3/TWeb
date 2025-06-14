using System.ComponentModel.DataAnnotations;

namespace TWeb.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        [Required]
        public int ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
        
        [Required]
        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser Sender { get; set; } = null!;
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}