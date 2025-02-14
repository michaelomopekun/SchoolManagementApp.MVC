using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApp.MVC.Models;
public class LoginView
{
    [Required]
    public string? Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}