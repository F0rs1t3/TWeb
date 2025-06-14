using System.ComponentModel.DataAnnotations;
using TWeb.Models;

namespace TWeb.Models.ViewModels
{
    public class StartChatViewModel
    {
        [Required]
        public int CarId { get; set; }
        
        [Required]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string InitialMessage { get; set; } = string.Empty;
        
        public ChatType ChatType { get; set; } = ChatType.General;
    }
}