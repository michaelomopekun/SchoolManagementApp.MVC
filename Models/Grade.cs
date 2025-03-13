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
        // public virtual CourseMaterial CourseMaterial { get; set; }

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

        public int GradePoint {get;set;}
            // get
            // {
            //     if (CreditHours == "1" && Score >= 80) return 5;
            //     if (CreditHours == "1" && Score >= 60) return 4;
            //     if (CreditHours == "1" && Score >= 50) return 3;
            //     if (CreditHours == "1" && Score >= 45) return 2;
            //     if (CreditHours == "1" && Score >= 40) return 1;

            // }
        // }

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

}