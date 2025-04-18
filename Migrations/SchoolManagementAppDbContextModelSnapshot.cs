﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    [DbContext(typeof(SchoolManagementAppDbContext))]
    partial class SchoolManagementAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Conversation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("LastMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastMessageAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("MessageCount")
                        .HasColumnType("int");

                    b.Property<string>("Topic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UnreadCount")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Conversations", (string)null);
                });

            modelBuilder.Entity("ConversationParticipant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ConversationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsMuted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsTyping")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime>("JoinedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<DateTime>("LastReadAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int?>("LastReadMessageId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastTypingAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("MutedUntil")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("LastReadMessageId");

                    b.HasIndex("UserId");

                    b.ToTable("ConversationParticipants", (string)null);
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AttachmentId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ConversationId")
                        .HasColumnType("int");

                    b.Property<int?>("ConversationParticipantId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DeletedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EditedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsEdited")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ReadAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ReceiverId")
                        .HasColumnType("int");

                    b.Property<int?>("ReplyToMessageId")
                        .HasColumnType("int");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SentAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("ConversationParticipantId");

                    b.HasIndex("ReplyToMessageId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages", (string)null);
                });

            modelBuilder.Entity("MessageAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MessageId")
                        .IsUnique();

                    b.ToTable("MessageAttachments", (string)null);
                });

            modelBuilder.Entity("MessageReaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MessageId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReactedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("Reaction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("MessageReactions", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.AcademicSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrentSemester")
                        .HasColumnType("int");

                    b.Property<string>("CurrentSession")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AcademicSettings");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Credit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LecturerId")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Semester")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LecturerId");

                    b.ToTable("Course", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.CourseMaterial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("FileContent")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("FileSize")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Semester")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("UploadDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("UploaderId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("UploaderId");

                    b.ToTable("CourseMaterials", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.CourseMaterialDownload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseMaterialId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DownloadDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseMaterialId");

                    b.HasIndex("StudentId");

                    b.ToTable("CourseMaterialDownloads", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Grade", b =>
                {
                    b.Property<int>("GradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GradeId"));

                    b.Property<string>("AcademicSession")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Comments")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreditHours")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GradePoint")
                        .HasColumnType("int");

                    b.Property<DateTime>("GradedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<decimal>("Score")
                        .HasPrecision(5, 2)
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("Semester")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GradeId");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("Grades", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("GeneratedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RecipientIdId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Permission", b =>
                {
                    b.Property<int>("permission_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("permission_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("permission_id"));

                    b.Property<string>("permission_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("permission_name");

                    b.HasKey("permission_id");

                    b.ToTable("Permissions", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Role", b =>
                {
                    b.Property<int>("role_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("role_id"));

                    b.Property<string>("role_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("role_name");

                    b.HasKey("role_id");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.RolePermission", b =>
                {
                    b.Property<int>("role_id")
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    b.Property<int>("permission_id")
                        .HasColumnType("int")
                        .HasColumnName("permission_id");

                    b.HasKey("role_id", "permission_id");

                    b.HasIndex("permission_id");

                    b.ToTable("Role_Permissions", (string)null);
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.UserCourse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EnrollmentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LecturerId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("WithdrawalDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("gradeStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId", "CourseId")
                        .IsUnique();

                    b.ToTable("UserCourses");
                });

            modelBuilder.Entity("Conversation", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.Course", "Course")
                        .WithMany("Conversations")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Course");
                });

            modelBuilder.Entity("ConversationParticipant", b =>
                {
                    b.HasOne("Conversation", "Conversation")
                        .WithMany("Participants")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Message", "LastReadMessage")
                        .WithMany()
                        .HasForeignKey("LastReadMessageId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "User")
                        .WithMany("Conversations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Conversation");

                    b.Navigation("LastReadMessage");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.HasOne("Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ConversationParticipant", "ConversationParticipant")
                        .WithMany()
                        .HasForeignKey("ConversationParticipantId");

                    b.HasOne("Message", "ReplyToMessage")
                        .WithMany()
                        .HasForeignKey("ReplyToMessageId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "Sender")
                        .WithMany("SentMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Conversation");

                    b.Navigation("ConversationParticipant");

                    b.Navigation("ReplyToMessage");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("MessageAttachment", b =>
                {
                    b.HasOne("Message", "Message")
                        .WithOne("Attachment")
                        .HasForeignKey("MessageAttachment", "MessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Message");
                });

            modelBuilder.Entity("MessageReaction", b =>
                {
                    b.HasOne("Message", "Message")
                        .WithMany("Reactions")
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "User")
                        .WithMany("MessageReactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Message");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Course", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.User", "Lecturer")
                        .WithMany("TaughtCourses")
                        .HasForeignKey("LecturerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Lecturer");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.CourseMaterial", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.Course", "Course")
                        .WithMany("CourseMaterials")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "Uploader")
                        .WithMany()
                        .HasForeignKey("UploaderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Uploader");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.CourseMaterialDownload", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.CourseMaterial", "CourseMaterial")
                        .WithMany("Downloads")
                        .HasForeignKey("CourseMaterialId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("CourseMaterial");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Grade", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.Course", "Course")
                        .WithMany("Grades")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "User")
                        .WithMany("Grades")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.RolePermission", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("permission_id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchoolManagementApp.MVC.Models.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.UserCourse", b =>
                {
                    b.HasOne("SchoolManagementApp.MVC.Models.Course", "Course")
                        .WithMany("EnrolledUsers")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SchoolManagementApp.MVC.Models.User", "User")
                        .WithMany("EnrolledCourses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Conversation", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Participants");
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.Navigation("Attachment");

                    b.Navigation("Reactions");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Course", b =>
                {
                    b.Navigation("Conversations");

                    b.Navigation("CourseMaterials");

                    b.Navigation("EnrolledUsers");

                    b.Navigation("Grades");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.CourseMaterial", b =>
                {
                    b.Navigation("Downloads");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.Role", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.User", b =>
                {
                    b.Navigation("Conversations");

                    b.Navigation("EnrolledCourses");

                    b.Navigation("Grades");

                    b.Navigation("MessageReactions");

                    b.Navigation("SentMessages");

                    b.Navigation("TaughtCourses");
                });
#pragma warning restore 612, 618
        }
    }
}
