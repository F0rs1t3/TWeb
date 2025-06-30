using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Services.Interfaces;

namespace TWeb.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IChatService chatService,
            UserManager<ApplicationUser> userManager,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Chat
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User is null in Chat Index");
                    return RedirectToAction("Login", "Account");
                }

                _logger.LogInformation("Loading chats for user: {UserId}", user.Id);
                var chats = await _chatService.GetUserChatsAsync(user.Id);
                _logger.LogInformation("Found {ChatCount} chats for user", chats.Count());

                return View(chats.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chat index for user: {UserId}", User?.Identity?.Name);
                TempData["Error"] = $"An error occurred while loading your chats. Details: {ex.Message}";
                return View(new List<ChatViewModel>());
            }
        }

        // GET: Chat/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (!await _chatService.CanUserAccessChatAsync(id, user.Id))
                {
                    return Forbid();
                }

                var chat = await _chatService.GetChatAsync(id, user.Id);
                if (chat == null)
                {
                    return NotFound();
                }

                // Mark messages as read
                await _chatService.MarkMessagesAsReadAsync(id, user.Id);

                return View(chat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading chat details for chat {ChatId}", id);
                TempData["Error"] = "An error occurred while loading the chat.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Chat/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(StartChatViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in Chat Start: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                TempData["Error"] = "Please provide a valid message.";
                return RedirectToAction("Details", "Cars", new { id = model.CarId });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User is null in Chat Start");
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Details", "Cars", new { id = model.CarId });
                }

                _logger.LogInformation("Starting chat for car {CarId} by user {UserId} with message: {Message}", model.CarId, user.Id, model.InitialMessage);

                var chatId = await _chatService.StartChatAsync(
                    model.CarId,
                    user.Id,
                    model.InitialMessage,
                    model.ChatType);

                _logger.LogInformation("Chat started successfully with ID: {ChatId}", chatId);
                TempData["Success"] = "Chat started successfully!";
                return RedirectToAction(nameof(Details), new { id = chatId });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ArgumentException in Chat Start: {Message}", ex.Message);
                TempData["Error"] = ex.Message;
                return RedirectToAction("Details", "Cars", new { id = model.CarId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting chat for car {CarId} by user {UserId}", model.CarId, User?.Identity?.Name);
                TempData["Error"] = $"An error occurred while starting the chat. Details: {ex.Message}";
                return RedirectToAction("Details", "Cars", new { id = model.CarId });
            }
        }

        // POST: Chat/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Message))
            {
                return Json(new { success = false, message = "Message cannot be empty." });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                if (!await _chatService.CanUserAccessChatAsync(model.ChatId, user.Id))
                {
                    return Json(new { success = false, message = "Access denied." });
                }

                var success = await _chatService.SendMessageAsync(model.ChatId, user.Id, model.Message);

                if (success)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to send message." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to chat {ChatId}", model.ChatId);
                return Json(new { success = false, message = "An error occurred while sending the message." });
            }
        }

        // GET: Chat/Messages/5
        public async Task<IActionResult> Messages(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                if (!await _chatService.CanUserAccessChatAsync(id, user.Id))
                {
                    return Json(new { success = false, message = "Access denied." });
                }

                var messages = await _chatService.GetChatMessagesAsync(id, user.Id);

                // Mark messages as read
                await _chatService.MarkMessagesAsReadAsync(id, user.Id);

                return Json(new { success = true, messages });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for chat {ChatId}", id);
                return Json(new { success = false, message = "An error occurred while loading messages." });
            }
        }

        // GET: Chat/UnreadCount
        public async Task<IActionResult> UnreadCount()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { count = 0 });
                }

                var count = await _chatService.GetUnreadMessageCountAsync(user.Id);
                return Json(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread message count");
                return Json(new { count = 0 });
            }
        }

        // GET: Chat/GetRecentChats
        public async Task<IActionResult> GetRecentChats()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                var chats = await _chatService.GetUserChatsAsync(user.Id);
                var recentChats = chats.Take(3).Select(chat => new
                {
                    id = chat.ChatId,
                    carTitle = chat.CarTitle ?? "Unknown Car",
                    lastMessage = chat.Messages?.LastOrDefault()?.Message ?? "No messages yet",
                    lastMessageTime = chat.Messages?.LastOrDefault()?.SentAt.ToString("MMM dd, HH:mm") ?? "",
                    hasUnread = chat.UnreadCount > 0,
                    unreadCount = chat.UnreadCount,
                    chatUrl = Url.Action("Details", "Chat", new { id = chat.ChatId })
                }).ToList();

                return Json(new { success = true, chats = recentChats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent chats for user: {UserId}", User?.Identity?.Name);
                return Json(new { success = false, message = "An error occurred while loading recent chats." });
            }
        }
    }
}
