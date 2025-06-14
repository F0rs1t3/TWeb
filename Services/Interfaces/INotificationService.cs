using TWeb.Models;

namespace TWeb.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId, int pageSize);
        Task<IEnumerable<Notification>> GetRecentNotificationsAsync(string userId, int count = 5);
        Task<int> GetUnreadCountAsync(string userId);
        Task<int> GetUnreadNotificationCountAsync(string userId);
        Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, string? actionUrl = null, string? relatedEntityId = null);
        Task<bool> MarkAsReadAsync(int notificationId, string userId);
        Task<bool> MarkNotificationAsReadAsync(int notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> MarkAllNotificationsAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(int notificationId, string userId);
    }
}