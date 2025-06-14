using TWeb.DTOs;
using TWeb.Models;

namespace TWeb.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task CreateNotificationAsync(NotificationDto notification);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<CarRental>> GetUpcomingRentalsAsync();
    }
}