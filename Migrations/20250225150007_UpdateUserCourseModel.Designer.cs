﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    [DbContext(typeof(SchoolManagementAppDbContext))]
    [Migration("20250225150007_UpdateUserCourseModel")]
    partial class UpdateUserCourseModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Credit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Course", (string)null);
                });

            modelBuilder.Entity("Grade", b =>
                {
                    b.Property<int>("GradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GradeId"));

                    b.Property<string>("Comments")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("GradedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<decimal>("Score")
                        .HasPrecision(5, 2)
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GradeId");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("Grades", (string)null);
                });

            modelBuilder.Entity("Permission", b =>
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

            modelBuilder.Entity("Role", b =>
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

            modelBuilder.Entity("RolePermission", b =>
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

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("WithdrawalDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("UserId");

                    b.ToTable("UserCourses");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

            modelBuilder.Entity("Grade", b =>
                {
                    b.HasOne("Course", "Course")
                        .WithMany("Grades")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("User", "User")
                        .WithMany("Grades")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("RolePermission", b =>
                {
                    b.HasOne("Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("permission_id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SchoolManagementApp.MVC.Models.UserCourse", b =>
                {
                    b.HasOne("Course", "Course")
                        .WithMany("EnrolledUsers")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("User", "User")
                        .WithMany("EnrolledCourses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Course", b =>
                {
                    b.Navigation("EnrolledUsers");

                    b.Navigation("Grades");
                });

            modelBuilder.Entity("Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("Role", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("EnrolledCourses");

                    b.Navigation("Grades");
                });
#pragma warning restore 612, 618
        }
    }
}
