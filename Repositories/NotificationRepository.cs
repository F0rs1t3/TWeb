using Microsoft.EntityFrameworkCore;
using TWeb.Data;
using TWeb.DTOs;
using TWeb.Models;
using TWeb.Repositories.Interfaces;

namespace TWeb.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(NotificationDto notificationDto)
        {
            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                Type = notificationDto.Type,
                RelatedEntityId = notificationDto.RelatedEntityId?.ToString(),
                ActionUrl = notificationDto.ActionUrl,
                CreatedAt = notificationDto.CreatedAt,
                IsRead = notificationDto.IsRead
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedEntityId = int.TryParse(n.RelatedEntityId, out var entityId) ? entityId : null,
                ActionUrl = n.ActionUrl,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            });
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int pageSize)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(pageSize)
                .ToListAsync();

            return notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                RelatedEntityId = int.TryParse(n.RelatedEntityId, out var entityId) ? entityId : null,
                ActionUrl = n.ActionUrl,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            });
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int notificationId, string userId)
        {
            try
            {
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

                if (notification == null)
                    return false;

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<IEnumerable<CarRental>> GetUpcomingRentalsAsync()
        {
            return await _context.CarRentals
                .Include(r => r.Car)
                .Where(r => r.Status == RentalStatus.Confirmed && r.StartDate >= DateTime.Today)
                .ToListAsync();
        }
    }
}