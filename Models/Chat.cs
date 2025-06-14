using System.ComponentModel.DataAnnotations;

namespace TWeb.Models
{
    public class Chat
    {
        public int Id { get; set; }
        
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;
        
        [Required]
        public string InitiatorId { get; set; } = string.Empty;
        public ApplicationUser Initiator { get; set; } = null!;
        
        [Required]
        public string ParticipantId { get; set; } = string.Empty;
        public ApplicationUser Participant { get; set; } = null!;
        
        public ChatType Type { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
    
    public enum ChatType
    {
        General,
        Purchase,
        Rental,
        Support
    }
}
