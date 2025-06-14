namespace TWeb.Models.ViewModels
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public int CarId { get; set; }
        public string CarTitle { get; set; } = string.Empty;
        public string? CarPhotoPath { get; set; }
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
        public bool IsOwner { get; set; }
        public int UnreadCount { get; set; }
        public DateTime LastMessageAt { get; set; }
        public List<ChatMessageViewModel> Messages { get; set; } = new();
    }
}
