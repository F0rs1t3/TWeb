using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TWeb.Models;
using TWeb.Services.Interfaces;

namespace TWeb.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Notification
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var notifications = await _notificationService.GetUserNotificationsAsync(user!.Id, 50);
                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                TempData["Error"] = "An error occurred while loading notifications.";
                return View(new List<Notification>());
            }
        }

        // GET: Notification/GetRecent
        public async Task<IActionResult> GetRecent()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var notifications = await _notificationService.GetUserNotificationsAsync(user!.Id, 10);
                var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(user.Id);
                
                return Json(new { 
                    notifications = notifications.Select(n => new {
                        id = n.Id,
                        title = n.Title,
                        message = n.Message,
                        type = n.Type.ToString(),
                        isRead = n.IsRead,
                        createdAt = n.CreatedAt.ToString("MMM dd, HH:mm"),
                        actionUrl = n.ActionUrl
                    }),
                    unreadCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent notifications");
                return Json(new { notifications = new object[0], unreadCount = 0 });
            }
        }

        // POST: Notification/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var success = await _notificationService.MarkNotificationAsReadAsync(id, user!.Id);
                
                return Json(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
                return Json(new { success = false });
            }
        }

        // POST: Notification/MarkAllAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var success = await _notificationService.MarkAllNotificationsAsReadAsync(user!.Id);
                
                return Json(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return Json(new { success = false });
            }
        }

        // POST: Notification/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var success = await _notificationService.DeleteNotificationAsync(id, user!.Id);
                
                if (success)
                {
                    TempData["Success"] = "Notification deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete notification.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
                TempData["Error"] = "An error occurred while deleting the notification.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Notification/UnreadCount
        public async Task<IActionResult> UnreadCount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var count = await _notificationService.GetUnreadNotificationCountAsync(user!.Id);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count");
                return Json(new { count = 0 });
            }
        }
    }
}