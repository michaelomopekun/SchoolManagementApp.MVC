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


        public NotificationService(INotificationRepository notificationRepository, IUserService userService, ILogger<NotificationHub> logger, IHubContext<NotificationHub> notificationHubContext)
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

        public async Task SendNotificationAsync(string Title, string userId, string message)
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
                    Title = Title,
                    Message = message,
                    RecipientIdId = user.Id,
                    GeneratedDate = DateTime.Now,
                    IsRead = false
                };

                await AddNotificationAsync(notification);

                // Send real-time notification
                await _notificationHubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification: {ex.Message}");
                throw;
            }
        }

        public async Task SendToAllUsers(string Title, string message)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                foreach (var user in users)
                {
                    await SendNotificationAsync(Title, user.Id.ToString(), message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to all users: {ex.Message}");
                throw;
            }
        }

        public async Task SendToAllStudents(string Title, string message)
        {
            try
            {
                var students = await _userService.GetAllStudentsAsync();

                foreach (var student in students)
                {
                    await SendNotificationAsync(Title, student.Id.ToString(), message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to all students: {ex.Message}");
                throw;
            }
        }

        public async Task SendToAllLecturers(string Title, string message)
        {
            try
            {
                var lecturers = await _userService.GetAllLecturerAsync();

                foreach (var lecturer in lecturers)
                {
                    await SendNotificationAsync(Title, lecturer.Id.ToString(), message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to all students: {ex.Message}");
                throw;
            }
        }

        public async Task SendToSpecificUsers(string title, string message, List<UserCourse> users)
        {
            try
            {
                foreach (var user in users)
                {
                    await SendNotificationAsync(title, user.Id.ToString(), message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to specific users: {ex.Message}");
                throw;
            }
        }
    }
}