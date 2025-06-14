using TWeb.Business.Interfaces;
using TWeb.DTOs;
using TWeb.Models;
using TWeb.Repositories.Interfaces;

namespace TWeb.Business
{
    public class NotificationBusinessLogic : INotificationBusinessLogic
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICarRepository _carRepository;
        private readonly ILogger<NotificationBusinessLogic> _logger;

        public NotificationBusinessLogic(
            INotificationRepository notificationRepository,
            ICarRepository carRepository,
            ILogger<NotificationBusinessLogic> logger)
        {
            _notificationRepository = notificationRepository;
            _carRepository = carRepository;
            _logger = logger;
        }

        public async Task CreateRentalRequestNotificationAsync(int rentalId, string ownerId)
        {
            var notification = new NotificationDto
            {
                UserId = ownerId,
                Type = NotificationType.RentalRequest,
                Title = "New Rental Request",
                Message = "You have received a new rental request for your car.",
                RelatedEntityId = rentalId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task CreateRentalConfirmationNotificationAsync(int rentalId, string renterId)
        {
            var notification = new NotificationDto
            {
                UserId = renterId,
                Type = NotificationType.RentalConfirmed,
                Title = "Rental Request Confirmed",
                Message = "Your rental request has been confirmed by the car owner.",
                RelatedEntityId = rentalId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepository.CreateNotificationAsync(notification);
        }

        public async Task CreateRentalReminderNotificationsAsync()
        {
            var upcomingRentals = await _notificationRepository.GetUpcomingRentalsAsync();
            
            foreach (var rental in upcomingRentals)
            {
                // Remind renter 24 hours before rental starts
                if (rental.StartDate.Date == DateTime.Today.AddDays(1))
                {
                    await CreateRentalReminderNotificationAsync(rental.Id, rental.RenterId, 
                        "Rental Starting Tomorrow", 
                        $"Your rental of {rental.Car.Brand} {rental.Car.Model} starts tomorrow.");
                }

                // Remind owner 24 hours before rental starts
                if (rental.StartDate.Date == DateTime.Today.AddDays(1))
                {
                    await CreateRentalReminderNotificationAsync(rental.Id, rental.Car.OwnerId, 
                        "Car Rental Starting Tomorrow", 
                        $"Your {rental.Car.Brand} {rental.Car.Model} rental starts tomorrow.");
                }

                // Remind renter 1 hour before rental ends
                if (rental.EndDate.Date == DateTime.Today && rental.EndDate.Hour == DateTime.Now.Hour + 1)
                {
                    await CreateRentalReminderNotificationAsync(rental.Id, rental.RenterId, 
                        "Rental Ending Soon", 
                        $"Your rental of {rental.Car.Brand} {rental.Car.Model} ends in 1 hour. Please return the car on time.");
                }
            }
        }

        public async Task CreateMaintenanceReminderNotificationsAsync()
        {
            var carsNeedingMaintenance = await _carRepository.GetCarsNeedingMaintenanceAsync();
            
            foreach (var car in carsNeedingMaintenance)
            {
                var notification = new NotificationDto
                {
                    UserId = car.OwnerId,
                    Type = NotificationType.MaintenanceReminder,
                    Title = "Maintenance Reminder",
                    Message = $"Your {car.Brand} {car.Model} may need maintenance. Current mileage: {car.Mileage:N0} miles.",
                    RelatedEntityId = car.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly = false)
        {
            return await _notificationRepository.GetUserNotificationsAsync(userId, unreadOnly);
        }

        public async Task MarkNotificationAsReadAsync(int notificationId, string userId)
        {
            await _notificationRepository.MarkAsReadAsync(notificationId, userId);
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        private async Task CreateRentalReminderNotificationAsync(int rentalId, string userId, string title, string message)
        {
            var notification = new NotificationDto
            {
                UserId = userId,
                Type = NotificationType.RentalReminder,
                Title = title,
                Message = message,
                RelatedEntityId = rentalId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepository.CreateNotificationAsync(notification);
        }
    }
}