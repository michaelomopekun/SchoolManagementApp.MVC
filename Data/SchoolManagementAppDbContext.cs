using Microsoft.EntityFrameworkCore;

public class SchoolManagementAppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    // public DbSet<Lecturer> Lecturer { get; set; }
    public DbSet<Course> Course { get; set; }

    public SchoolManagementAppDbContext(DbContextOptions<SchoolManagementAppDbContext> options) : base(options){}

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

        // modelBuilder.Entity<Lecturer>(entity =>
        // {
        //     entity.HasKey(e => e.Id);
        //     entity.HasIndex(e => e.Username).IsUnique();
        //     entity.Property(e => e.Role)
        //         .HasConversion<string>()
        //         .HasMaxLength(50)
        //         .IsRequired();
        // });

        modelBuilder.Entity<Course>(entity => {
            entity.HasKey(e => e.Id); // Treat Course as a regular entity with a key
        });
    }
}