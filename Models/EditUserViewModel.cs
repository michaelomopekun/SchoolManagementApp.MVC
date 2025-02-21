using System.ComponentModel.DataAnnotations;
using SchoolManagementApp.MVC.Models;

public class EditUserViewModel
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public UserRole Role { get; set; }
}