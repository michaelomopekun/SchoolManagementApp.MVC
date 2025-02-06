using System.ComponentModel.DataAnnotations;

namespace SchoolManagementApp.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}