using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Repository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
        Task<Notification> GetNotificationByIdAsync(int notificationId);
        Task AddNotificationAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task DeleteNotificationAsync(int notificationId);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
        Task MarkAllNotificationsAsReadAsync(int userId);
        Task<bool> MarkNotificationAsReadAsync(int notificationId);
    }
}