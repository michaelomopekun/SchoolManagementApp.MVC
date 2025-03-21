using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementApp.MVC.Models;

namespace SchoolManagementApp.MVC.Models
{
    public class User : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public Level Level { get; set; }

        public virtual ICollection<UserCourse> EnrolledCourses { get; set; } = new List<UserCourse>();
        public virtual ICollection<Grade>? Grades { get; set; }
        public virtual ICollection<Course>? TaughtCourses { get; set; }
        public virtual ICollection<ConversationParticipant>? Conversations { get; set; }
        public virtual ICollection<Message>? SentMessages { get; set; }
        public virtual ICollection<MessageReaction>? MessageReactions { get; set; }
    }

    public class RolePermission
    {
        [Key]
        [Column("role_id")]
        public int role_id { get; set; }
        [Key]
        [Column("permission_id")]
        public int permission_id { get; set; }

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
    public class Permission
    {
        [Key]
        [Column("permission_id")]
        public int permission_id { get; set; }

        [Column("permission_name")]
        public string permission_name { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }

    public class Role
    {
        [Key]
        [Column("role_id")]
        public int role_id { get; set; }

        [Column("role_name")]
        public string role_name { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }

    public enum Level
    {
        LevelNoneStudent = 0,
        Level100 = 100,
        Level200 = 200,
        Level300 = 300,
        Level400 = 400,
        Level500 = 500
    }

}