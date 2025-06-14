using TWeb.Models;
using TWeb.Models.ViewModels;

namespace TWeb.Services.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatViewModel>> GetUserChatsAsync(string userId);
        Task<ChatViewModel?> GetChatDetailsAsync(int chatId, string userId);
        Task<ChatViewModel?> GetChatAsync(int chatId, string userId);
        Task<IEnumerable<ChatMessageViewModel>> GetChatMessagesAsync(int chatId, string userId);
        Task<int> StartChatAsync(StartChatViewModel model, string userId);
        Task<int> StartChatAsync(int carId, string userId, string initialMessage, ChatType chatType);
        Task<bool> SendMessageAsync(SendMessageViewModel model, string userId);
        Task<bool> SendMessageAsync(int chatId, string userId, string message);
        Task<bool> CanUserAccessChatAsync(int chatId, string userId);
        Task<int> GetUnreadMessageCountAsync(string userId);
        Task<bool> MarkMessagesAsReadAsync(int chatId, string userId);
    }
}