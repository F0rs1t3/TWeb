using TWeb.DTOs;

namespace TWeb.Business.Interfaces
{
    public interface INotificationBusinessLogic
    {
        Task CreateRentalRequestNotificationAsync(int rentalId, string ownerId);
        Task CreateRentalConfirmationNotificationAsync(int rentalId, string renterId);
        Task CreateRentalReminderNotificationsAsync();
        Task CreateMaintenanceReminderNotificationsAsync();
        Task CreateNewCarListedNotificationAsync(int carId, string ownerId);
        Task CreateCarDeletedNotificationAsync(int carId, string ownerId, string reason);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int pageSize);
        Task MarkNotificationAsReadAsync(int notificationId, string userId);
        Task<bool> MarkAllNotificationsAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(int notificationId, string userId);
        Task<int> GetUnreadNotificationCountAsync(string userId);
    }
}