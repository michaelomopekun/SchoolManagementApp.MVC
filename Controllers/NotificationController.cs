using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementApp.MVC.Services;

namespace SchoolManagementApp.MVC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized();
                
            var userId = int.Parse(userClaim.Value);
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("MarkAsRead/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok();
        }

        [HttpPost("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized();
                
            var userId = int.Parse(userClaim.Value);
            await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok();
        }

        [HttpGet("UnreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized();
                
            var userId = int.Parse(userClaim.Value);
            var count = await _notificationService.GetUnreadNotificationsCountAsync(userId);
            return Ok(count);
        }

        [HttpDelete("Delete/{notificationId}")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok();
        }

        [HttpGet("GetNotification/{notificationId}")]
        public async Task<IActionResult> GetNotification(int notificationId)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
            return Ok(notification);
        }
    }
}