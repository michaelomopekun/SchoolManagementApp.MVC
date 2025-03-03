using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementApp.MVC.Models
{
    public class UserCourse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; }
        
        [Required]
        public EnrollmentStatus Status { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        public DateTime WithdrawalDate{get;set;}
        public int LecturerId { get; set; }
    }

    public enum EnrollmentStatus
    {
        Active= 1,
        Completed= 2,
        Withdrawn= 0
    }
}