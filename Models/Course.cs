// namespace SchoolManagementApp.MVC.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Course
{
     [Key]
     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }
    [Required(ErrorMessage = "Course Code is required")]
    public string? Code { get; set; }
    [Required(ErrorMessage = "Course Name is required")]
    public string? Name { get; set; }
    public string? Credit { get; set; }
    public string? Description { get; set; }

    public ICollection<User>? users { get; set; } = new List<User>();
}
