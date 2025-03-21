using Microsoft.EntityFrameworkCore;
using SchoolManagementApp.MVC.Models;

public class SchoolManagementAppDbContext : DbContext
{

    public SchoolManagementAppDbContext()
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Course { get; set; }
    public DbSet<RolePermission> Role_Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserCourse> UserCourses { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<CourseMaterial> CourseMaterials { get; set; }
    public DbSet<CourseMaterialDownload> CourseMaterialDownloads { get; set; }
    public DbSet<AcademicSetting> AcademicSettings { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<MessageReaction> MessageReactions { get; set; }
    // public DbSet<GradeReport> GradeReports { get; set; }

    public SchoolManagementAppDbContext(DbContextOptions<SchoolManagementAppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SchoolManagementAppDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }

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

            // // configuration for CourseMaterials
            // entity.HasMany(c => c.CourseMaterial)
            //     .WithOne(cm => cm.Course)
            //     .HasForeignKey(cm => cm.CourseId)
            //     .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<CourseMaterial>(entity =>
        {
            entity.ToTable("CourseMaterials");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UploadDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            // entity.Property(e => e.Uploader).IsRequired();

            entity.HasOne(e => e.Course)
                .WithMany(c => c.CourseMaterials)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Uploader)
                .WithMany()
                .HasForeignKey(u => u.UploaderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CourseMaterialDownload>(entity =>
        {
            entity.ToTable("CourseMaterialDownloads");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.StudentId).IsRequired();
            entity.Property(e => e.DownloadDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.CourseMaterial)
                .WithMany(d => d.Downloads)
                .HasForeignKey(c => c.CourseMaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.ToTable("Conversations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Conversations)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConversationParticipant>(entity =>
        {
            entity.ToTable("ConversationParticipants");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ConversationId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.JoinedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.LastReadAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.IsTyping).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.IsMuted).IsRequired().HasDefaultValue(false);

            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(c => c.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.LastReadMessage)
                .WithMany()
                .HasForeignKey(e => e.LastReadMessageId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.SentAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.IsEdited).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.Status).IsRequired().HasDefaultValue(MessageStatus.Sent);

            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(c => c.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Sender)
                .WithMany(e => e.SentMessages)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReplyToMessage)
                .WithMany()
                .HasForeignKey(e => e.ReplyToMessageId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.ToTable("MessageReactions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Reaction).IsRequired();
            entity.Property(e => e.ReactedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                .WithMany(u => u.MessageReactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.ToTable("MessageAttachments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Size).IsRequired();

            entity.HasOne(e => e.Message)
                .WithOne(m => m.Attachment)
                .HasForeignKey<MessageAttachment>(e => e.MessageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }
}