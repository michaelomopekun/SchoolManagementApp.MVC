// namespace SchoolManagementApp.MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Models
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Course Code is required")]
        public string? Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course Name is required")]
        [Display(Name = "Course Name")]
        public string Name { get; set; } = string.Empty;
        public string? Credit { get; set; }
        public string? Description { get; set; } = string.Empty;
        public int LecturerId { get; set; }



    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; }

        public virtual User? Lecturer { get; set; }
        public virtual ICollection<UserCourse>? EnrolledUsers { get; set; } = new List<UserCourse>();
        public virtual ICollection<Grade>? Grades { get; set; }
    }

    public class CourseMaterial
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int UploaderId { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
        public string FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string Description { get; set; }


        // public virtual CourseMaterial CourseMaterial { get; set; }
        public virtual Course Course { get; set; }
        public virtual User Uploader { get; set; }
        public virtual ICollection<CourseMaterialDownload> Downloads { get; set; }
    }

    public class CourseMaterialDownload
    {
        public int Id { get; set; }
        public int CourseMaterialId { get; set; }
        public int StudentId { get; set; }
        public DateTime DownloadDate { get; set; }


        public virtual CourseMaterial CourseMaterial { get; set; }
        public virtual User Student { get; set; }
    }

}