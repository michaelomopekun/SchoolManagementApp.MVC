using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int notificationId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(int userId);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task SendNotificationAsync(string userId, string message);
    }
}