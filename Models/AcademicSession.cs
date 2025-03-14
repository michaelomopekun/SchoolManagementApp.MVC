using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApp.MVC.Models
{
    public class AcademicSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Current Academic Session")]
        public string CurrentSession { get; set; }

        [Required]
        [Display(Name = "Current Semester")]
        public Semester CurrentSemester { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}