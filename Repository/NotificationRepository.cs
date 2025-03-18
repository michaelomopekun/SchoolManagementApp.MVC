using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;


namespace SchoolManagementApp.MVC.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly SchoolManagementAppDbContext _context;

        public NotificationRepository(SchoolManagementAppDbContext context)
        {
            _context = context;
        }


        public async Task AddNotificationAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if(notification == null)
            {
                return;
            }
            _context.Notifications.Remove(notification);
            
            await _context.SaveChangesAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if(notification == null)
            {
                return null;
            }
            return notification;
        }

        public async  Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
        {
            var user_notification = await _context.Notifications
                .Where(n => n.RecipientIdId == userId)
                .OrderByDescending(n => n.GeneratedDate).ToListAsync();

            return user_notification;
        }

        public Task<int> GetUnreadNotificationsCountAsync(int userId)
        {
            var unRead = _context.Notifications
                .Where( u => u.RecipientIdId == userId && u.IsRead == false)
                .CountAsync();

            return unRead;
        }

        public async Task MarkAllNotificationsAsReadAsync(int userId)
        {
            var userNotification = await _context.Notifications
                .Where(n => n.RecipientIdId == userId && n.IsRead ==false)
                .ToListAsync();

            if(userNotification != null)
            {
                foreach(var notification in userNotification)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);

            if(notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public Task UpdateNotificationAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

    }
}