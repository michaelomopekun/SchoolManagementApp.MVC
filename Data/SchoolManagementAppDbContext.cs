using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;

public class SchoolManagementAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Course { get; set; }
    public DbSet<RolePermission> Role_Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<Grade> Grades { get; set; }

    public SchoolManagementAppDbContext(DbContextOptions<SchoolManagementAppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Role)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        });

        modelBuilder.Entity<UserCourse>(entity =>
        {
           entity.HasKey(e => e.Id);

            // Configure Id as identity column
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            // Add unique constraint for UserId and CourseId combination
            entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();

            entity.HasOne(uc => uc.User)
                .WithMany(u => u.EnrolledCourses)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(uc => uc.Course)
                .WithMany(c => c.EnrolledUsers)
                .HasForeignKey(uc => uc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // entity.HasOne(uc => uc.User)
            //     .WithMany(u => u.TaughtCourses)
            //     .HasForeignKey(uc => uc.LecturerId)
            //     .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("Role_Permissions"); // Match your existing table name
            entity.HasKey(rp => new { rp.role_id, rp.permission_id });

            entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.role_id)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.permission_id)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(r => r.role_id);
            entity.Property(r => r.role_name).IsRequired();
        });

        // Permission configuration
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(p => p.permission_id);
            entity.Property(p => p.permission_name).IsRequired();
        });

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");
            entity.HasKey(e => e.Id);
            entity.Property(l => l.LecturerId).IsRequired();

            entity.HasOne(c => c.Lecturer)
                .WithMany(l => l.TaughtCourses)
                .HasForeignKey(c => c.LecturerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Grade configuration
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grades");
            entity.HasKey(e => e.GradeId);

            entity.HasOne(g => g.User)
                .WithMany(u => u.Grades)
                .HasForeignKey(g => g.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(g => g.Score)
                .IsRequired()
                .HasPrecision(5, 2);

            entity.Property(g => g.GradedDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}