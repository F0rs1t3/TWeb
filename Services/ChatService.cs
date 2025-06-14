using Microsoft.EntityFrameworkCore;
using TWeb.Data;
using TWeb.Models;
using TWeb.Models.ViewModels;
using TWeb.Services.Interfaces;

namespace TWeb.Services
{
    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChatService> _logger;

        public ChatService(ApplicationDbContext context, ILogger<ChatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ChatViewModel>> GetUserChatsAsync(string userId)
        {
            try
            {
                var chats = await _context.Chats
                    .Include(c => c.Car)
                    .Include(c => c.Initiator)
                    .Include(c => c.Participant)
                    .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                    .Where(c => c.InitiatorId == userId || c.ParticipantId == userId)
                    .OrderByDescending(c => c.LastMessageAt)
                    .ToListAsync();

                return chats.Select(chat => new ChatViewModel
                {
                    ChatId = chat.Id,
                    CarId = chat.CarId,
                    CarTitle = $"{chat.Car.Brand} {chat.Car.Model} ({chat.Car.Year})",
                    CarPhotoPath = chat.Car.PhotoPath,
                    OtherUserId = chat.InitiatorId == userId ? chat.ParticipantId : chat.InitiatorId,
                    OtherUserName = chat.InitiatorId == userId 
                        ? $"{chat.Participant.FirstName} {chat.Participant.LastName}".Trim()
                        : $"{chat.Initiator.FirstName} {chat.Initiator.LastName}".Trim(),
                    IsOwner = chat.Car.OwnerId == userId,
                    UnreadCount = chat.Messages.Count(m => !m.IsRead && m.SenderId != userId),
                    LastMessageAt = chat.LastMessageAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user chats for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<ChatViewModel?> GetChatDetailsAsync(int chatId, string userId)
        {
            return await GetChatAsync(chatId, userId);
        }

        public async Task<ChatViewModel?> GetChatAsync(int chatId, string userId)
        {
            try
            {
                var chat = await _context.Chats
                    .Include(c => c.Car)
                    .Include(c => c.Initiator)
                    .Include(c => c.Participant)
                    .Include(c => c.Messages)
                        .ThenInclude(m => m.Sender)
                    .FirstOrDefaultAsync(c => c.Id == chatId && 
                                           (c.InitiatorId == userId || c.ParticipantId == userId));

                if (chat == null)
                    return null;

                // Mark messages as read
                var unreadMessages = chat.Messages.Where(m => !m.IsRead && m.SenderId != userId);
                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                }
                await _context.SaveChangesAsync();

                return new ChatViewModel
                {
                    ChatId = chat.Id,
                    CarId = chat.CarId,
                    CarTitle = $"{chat.Car.Brand} {chat.Car.Model} ({chat.Car.Year})",
                    CarPhotoPath = chat.Car.PhotoPath,
                    OtherUserId = chat.InitiatorId == userId ? chat.ParticipantId : chat.InitiatorId,
                    OtherUserName = chat.InitiatorId == userId 
                        ? $"{chat.Participant.FirstName} {chat.Participant.LastName}".Trim()
                        : $"{chat.Initiator.FirstName} {chat.Initiator.LastName}".Trim(),
                    IsOwner = chat.Car.OwnerId == userId,
                    Messages = chat.Messages.OrderBy(m => m.SentAt).Select(m => new ChatMessageViewModel
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}".Trim(),
                        Message = m.Message,
                        SentAt = m.SentAt,
                        IsRead = m.IsRead,
                        IsCurrentUser = m.SenderId == userId
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat details for chat: {ChatId}", chatId);
                throw;
            }
        }

        public async Task<IEnumerable<ChatMessageViewModel>> GetChatMessagesAsync(int chatId, string userId)
        {
            try
            {
                if (!await CanUserAccessChatAsync(chatId, userId))
                    return new List<ChatMessageViewModel>();

                var messages = await _context.ChatMessages
                    .Include(m => m.Sender)
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                return messages.Select(m => new ChatMessageViewModel
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}".Trim(),
                    Message = m.Message,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                    IsCurrentUser = m.SenderId == userId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat messages for chat: {ChatId}", chatId);
                throw;
            }
        }

        public async Task<int> StartChatAsync(StartChatViewModel model, string userId)
        {
            return await StartChatAsync(model.CarId, userId, model.InitialMessage, model.ChatType);
        }

        public async Task<int> StartChatAsync(int carId, string userId, string initialMessage, ChatType chatType)
        {
            try
            {
                var car = await _context.Cars.FindAsync(carId);
                if (car == null)
                    throw new ArgumentException("Car not found");

                if (car.OwnerId == userId)
                    throw new ArgumentException("Cannot start chat with yourself");

                // Check if chat already exists
                var existingChat = await _context.Chats
                    .FirstOrDefaultAsync(c => c.CarId == carId &&
                                            ((c.InitiatorId == userId && c.ParticipantId == car.OwnerId) ||
                                             (c.InitiatorId == car.OwnerId && c.ParticipantId == userId)));

                if (existingChat != null)
                    return existingChat.Id;

                // Create new chat
                var chat = new Chat
                {
                    CarId = carId,
                    InitiatorId = userId,
                    ParticipantId = car.OwnerId,
                    Type = chatType
                };

                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();

                // Send initial message
                var message = new ChatMessage
                {
                    ChatId = chat.Id,
                    SenderId = userId,
                    Message = initialMessage
                };

                _context.ChatMessages.Add(message);
                chat.LastMessageAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return chat.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting chat for car: {CarId}", carId);
                throw;
            }
        }

        public async Task<bool> SendMessageAsync(SendMessageViewModel model, string userId)
        {
            return await SendMessageAsync(model.ChatId, userId, model.Message);
        }

        public async Task<bool> SendMessageAsync(int chatId, string userId, string message)
        {
            try
            {
                var chat = await _context.Chats
                    .FirstOrDefaultAsync(c => c.Id == chatId && 
                                           (c.InitiatorId == userId || c.ParticipantId == userId));

                if (chat == null)
                    return false;

                var chatMessage = new ChatMessage
                {
                    ChatId = chatId,
                    SenderId = userId,
                    Message = message
                };

                _context.ChatMessages.Add(chatMessage);
                chat.LastMessageAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to chat: {ChatId}", chatId);
                throw;
            }
        }

        public async Task<bool> CanUserAccessChatAsync(int chatId, string userId)
        {
            try
            {
                return await _context.Chats
                    .AnyAsync(c => c.Id == chatId && 
                                  (c.InitiatorId == userId || c.ParticipantId == userId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking chat access for chat: {ChatId}", chatId);
                return false;
            }
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            try
            {
                return await _context.ChatMessages
                    .Include(m => m.Chat)
                    .Where(m => !m.IsRead && 
                               m.SenderId != userId &&
                               (m.Chat.InitiatorId == userId || m.Chat.ParticipantId == userId))
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread message count for user: {UserId}", userId);
                return 0;
            }
        }

        public async Task<bool> MarkMessagesAsReadAsync(int chatId, string userId)
        {
            try
            {
                var messages = await _context.ChatMessages
                    .Include(m => m.Chat)
                    .Where(m => m.ChatId == chatId && 
                               !m.IsRead && 
                               m.SenderId != userId &&
                               (m.Chat.InitiatorId == userId || m.Chat.ParticipantId == userId))
                    .ToListAsync();

                foreach (var message in messages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking messages as read for chat: {ChatId}", chatId);
                return false;
            }
        }
    }
}
