using Microsoft.AspNetCore.SignalR;
using SchoolManagementApp.MVC.Models;
using SchoolManagementApp.MVC.Services;

namespace SchoolManagementApp.MVC.Hubs
{
    public class NotificationHub : Hub
    {
    private readonly ILogger<NotificationHub> _logger;
    private readonly INotificationService _notificationService;

    public NotificationHub(ILogger<NotificationHub> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;

    }

        public async Task SendNotification(string userId, string message)
        {
            try
            {
                var notification = new Notification
                {
                    Title = "New Notification",
                    RecipientIdId = int.Parse(userId),
                    Message = message,
                    GeneratedDate = DateTime.Now,
                    IsRead = false
                };

                await _notificationService.AddNotificationAsync(notification);
                await Clients.User(userId).SendAsync("ReceiveNotification", message);

                _logger.LogWarning("--------------------------------------------------------------------------------------------------------------------------------------------------------");
                _logger.LogWarning($"SendNotification : Successfully sent notification to user: {userId}. ");
                _logger.LogWarning("--------------------------------------------------------------------------------------------------------------------------------------------------------");
            
            }
            catch (Exception ex)
            {
                _logger.LogWarning("--------------------------------------------------------------------------------------------------------------------------------------------------------");
                _logger.LogWarning($"SendNotification : Error sending notification: {ex.Message}, to user: {userId}. ");
                _logger.LogWarning("--------------------------------------------------------------------------------------------------------------------------------------------------------");
            }
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            // await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}