using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagementApp.MVC.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required(ErrorMessage = "Course is required")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        [Required]
        public string CourseName {get;set;}

        [Required]
        public string CourseCode {get; set;}

        [Required]
        public string CreditHours {get; set;}

        [Required]
        public Semester Semester { get; set; } 

        // [Required]
        // public DateTime GeneratedDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string AcademicSession { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public decimal? Score { get; set; }
        public string LetterGrade
        {
            get
            {
                if(Score == null) return "N/A";
                var score = Score.Value;
                if(score >= 80) return "A";
                if(score >= 60) return "B";
                if(score >= 50) return "C";
                if(score >= 45) return "D";
                return "F";
            }
        }

        [MaxLength(500)]
        public string? Comments { get; set; }

        public DateTime GradedDate { get; set; }

        public Grade ToDto()
        {
            return new Grade
            {
                // User = User,
                UserId = UserId,
                CourseId = CourseId,
                CourseCode = CourseCode,
                CreditHours = CreditHours,
                Score = Score,
                Comments = Comments,
                
                Semester = Semester,
                // GeneratedDate = GeneratedDate,
                AcademicSession = AcademicSession
            };
        }
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

    public enum Semester
    {
        FirstSemester = 1,
        SecondSemester = 2
    }

    public enum AcademicSession
    {
        
    }

    // public class GradeReport
    // {
    //     [Key]
    //     public int Id { get; set; }

    //     [Required]
    //     public int CourseId { get; set; }
    //     public virtual Course Course { get; set; }

    //     [Required]
    //     public int StudentId { get; set; }
    //     public virtual User Student { get; set; }

    //     [Required]
    //     public int GradeId { get; set; }
    //     public virtual Grade Grade { get; set; }
    //     public string Comments { get; set; }

    //     [Required]
    //     [MaxLength(10)]
    //     public string AcademicSession { get; set; }

    //     [Required]
    //     public Semester Semester { get; set; } 

    //     public DateTime GeneratedDate { get; set; }

    //     [NotMapped]
    //     public string LetterGrade
    //     {
    //         get
    //         {
    //             if(Grade?.Score == null) return "N/A";
    //             var score = Grade.Score.Value;
    //             if(score >= 80) return "A";
    //             if(score >= 60) return "B";
    //             if(score >= 50) return "C";
    //             if(score >= 45) return "D";
    //             return "F";
    //         }
    //     }

    //     public Grade ToDto()
    //     {
    //         return new Grade
    //         {
    //             // User = User,
    //             UserId = Grade.UserId,
    //             CourseId = Grade.CourseId,
    //             CourseCode = Grade.CourseCode,
    //             CreditHours = Grade.CreditHours,
    //             Score = Grade.Score,
    //             Comments = Grade.Comments,
    //             // LetterGrade is read-only and cannot be assigned
    //             Semester = Grade.Semester,
    //             GeneratedDate = Grade.GeneratedDate,
    //             AcademicSession = Grade.AcademicSession
    //         };
    //     }
    // }


    // public class GradeReportDto
    // {
    //     [Display(Name = "Student Name")]
    //     public string StudentName { get; set; }

    //     [Display(Name = "Student ID")]
    //     public string StudentId { get; set; }

    //     [Display(Name = "Course")]
    //     public string CourseName { get; set; }

    //     [Display(Name = "Course Code")]
    //     public string CourseCode { get; set; }

    //     [Display(Name = "Credit Hours")]
    //     public string CreditHours { get; set; }

    //     [Display(Name = "Score")]
    //     public decimal? Score { get; set; }

    //     [Display(Name = "Grade")]
    //     public string LetterGrade { get; set; }

    //     [Display(Name = "Semester")]
    //     public string Semester { get; set; }

    //     [Display(Name = "Date Generated")]
    //     [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
    //     public DateTime GeneratedDate { get; set; }

    //     [Display(Name = "Academic Session")]
    //     public string AcademicSession { get; set; }

    //     [Display(Name = "Comments")]
    //     public string Comment { get; set; }

    // }
}