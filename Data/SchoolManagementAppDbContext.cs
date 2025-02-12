using Microsoft.EntityFrameworkCore;

public class SchoolManagementAppDbContext : DbContext
{
    public DbSet<Student> Student { get; set; }
    public DbSet<Lecturers> Lecturers { get; set; }
    public DbSet<Course> Course { get; set; }

    public SchoolManagementAppDbContext(DbContextOptions<SchoolManagementAppDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // modelBuilder.Entity<Course>().HasNoKey();  // ✅ Treat Course as a keyless entity
    modelBuilder.Entity<Course>().HasKey(c=>c.Id); // ✅ Treat Course as a regular entity with a key
    base.OnModelCreating(modelBuilder);
}

}