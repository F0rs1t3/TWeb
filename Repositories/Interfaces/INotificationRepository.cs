using TWeb.DTOs;
using TWeb.Models;

namespace TWeb.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task CreateNotificationAsync(NotificationDto notification);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int pageSize);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteAsync(int notificationId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<CarRental>> GetUpcomingRentalsAsync();
    }
}