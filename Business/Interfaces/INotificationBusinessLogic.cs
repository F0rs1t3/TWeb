using TWeb.DTOs;

namespace TWeb.Business.Interfaces
{
    public interface INotificationBusinessLogic
    {
        Task CreateRentalRequestNotificationAsync(int rentalId, string ownerId);
        Task CreateRentalConfirmationNotificationAsync(int rentalId, string renterId);
        Task CreateRentalReminderNotificationsAsync();
        Task CreateMaintenanceReminderNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
        Task MarkNotificationAsReadAsync(int notificationId, string userId);
        Task<int> GetUnreadNotificationCountAsync(string userId);
    }
}