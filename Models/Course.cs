// namespace SchoolManagementApp.MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApp.MVC.Models;

public class Course
{
     [Key]
     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required(ErrorMessage = "Course Code is required")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course Name is required")]
    [Display(Name = "Course Name")]
    public string Name { get; set; } = string.Empty;
    public string Credit { get; set; }
    public string Description { get; set; } = string.Empty;

    public ICollection<UserCourse>? EnrolledUsers { get; set; } = new List<UserCourse>();
    public virtual ICollection<Grade> Grades { get; set; }
}
