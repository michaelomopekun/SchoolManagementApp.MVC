using Microsoft.EntityFrameworkCore;

public class SchoolManagementAppDbContext : DbContext
{
    public DbSet<Student> Student { get; set; }
    public DbSet<Lecturers> Lecturers { get; set; }
    public DbSet<Course> Course { get; set; }

    public SchoolManagementAppDbContext(DbContextOptions<SchoolManagementAppDbContext> options) : base(options){}
}