
namespace SchoolManagementApp.MVC.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int RecipientIdId { get; set; }
        // public User User { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
    }
}
