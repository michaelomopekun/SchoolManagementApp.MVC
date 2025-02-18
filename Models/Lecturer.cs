// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using SchoolManagementApp.MVC.Models;

// public class Lecturer : IUser
// {
//     [Key]
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     public int Id { get; set; }  

//     [Required]
//     [StringLength(50)]
//     public string Username { get; set; }

//     [Required]
//     public string Password { get; set; }

//     [Required]
//     public UserRole Role { get; set; }
// }