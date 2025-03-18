using Microsoft.AspNetCore.SignalR;
using SchoolManagementApp.MVC.Hubs;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Repository;


namespace SchoolManagementApp.MVC.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserService _userService;
        private readonly ILogger<NotificationHub> _logger;
        private readonly IHubContext<NotificationHub> _notificationHubContext;


        public NotificationService(INotificationRepository notificationRepository, IUserService userService,ILogger<NotificationHub> logger,IHubContext<NotificationHub> notificationHubContext)
        {
            _notificationRepository = notificationRepository;
            _userService = userService;
            _logger = logger;
            _notificationHubContext = notificationHubContext;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _notificationRepository.AddNotificationAsync(notification);
        }
        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
        {
            return await _notificationRepository.GetNotificationsForUserAsync(userId);
        }
        public async Task MarkAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkNotificationAsReadAsync(notificationId);
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            await _notificationRepository.DeleteNotificationAsync(notificationId);
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            return await _notificationRepository.GetNotificationByIdAsync(notificationId);
        }


        public async Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadNotificationsCountAsync(userId);
        }

        public async Task MarkAllNotificationsAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllNotificationsAsReadAsync(userId);
        }


        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkNotificationAsReadAsync(notificationId);
        }

        public Task UpdateNotificationAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

        public async Task SendNotificationAsync(string userId, string message)
        {
            try 
            {
                // Verify user exists
                var user = await _userService.GetUserByIdAsync(int.Parse(userId));
                if (user == null)
                {
                    _logger.LogError($"Failed to send notification: User {userId} not found");
                    return;
                }

                // Create notification entity
                var notification = new Notification
                {
                    Title = "Grade Update",
                    Message = message,
                    RecipientIdId = user.Id, // Use the verified user ID
                    GeneratedDate = DateTime.Now,
                    IsRead = false
                };

                // Save to database
                await AddNotificationAsync(notification);

                // Send real-time notification
                await _notificationHubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification: {ex.Message}");
                // Consider whether to rethrow or handle silently
                throw;
            }
        }
    }
}