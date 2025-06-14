using System.ComponentModel.DataAnnotations;

namespace TWeb.Models.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public int ChatId { get; set; }
        
        [Required]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;
    }
}