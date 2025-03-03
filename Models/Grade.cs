using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApp.MVC.Models
{
    public class Grade
    {
        [Key]
        public int? GradeId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public decimal? Score { get; set; }

        [MaxLength(500)]
        public string? Comments { get; set; }

        public DateTime GradedDate { get; set; }
    }



    public class GradeListViewModel
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public List<Grade>? Grades { get; set; }
    }



    public class GradeFilterViewModel
    {
        public int? StudentId { get; set; }
        public GradeStatus? GradeStatus { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? MinScore { get; set; }
        public int CourseId { get; set; }
    }

    public enum GradeStatus
    {
        All,
        Graded,
        NotGraded
    }
}