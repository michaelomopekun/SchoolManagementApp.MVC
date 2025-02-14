using Microsoft.EntityFrameworkCore;

public class SchoolManagementAppDbContext : DbContext
{
    public DbSet<Student> Student { get; set; }
    public DbSet<Lecturers> Lecturers { get; set; }
    public DbSet<Course> Course { get; set; }

    public SchoolManagementAppDbContext(DbContextOptions<SchoolManagementAppDbContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Student>(entity =>{
        entity.HasKey(c => c.Id); // ✅ Treat Course as a regular entity with a key
        entity.Property(c => c.Id).ValueGeneratedOnAdd(); // ✅ Treat Course.Id as an auto-generated value
        entity.Property(c => c.Username).IsRequired(); // ✅ Treat Course.Username as a required property
        entity.Property(c => c.Password).IsRequired(); // ✅ Treat Course.Password as a required property
        entity.HasIndex(c => c.Username).IsUnique(); // ✅ Treat Course.Username as a unique property
    });

    modelBuilder.Entity<Course>(entity => {
        entity.HasKey(e => e.Id); // ✅ Treat Course as a regular entity with a key
    });
    base.OnModelCreating(modelBuilder);
}

}