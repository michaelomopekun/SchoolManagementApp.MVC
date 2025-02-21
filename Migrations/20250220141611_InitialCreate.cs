using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

        //     migrationBuilder.CreateTable(
        //         name: "Permissions",
        //         columns: table => new
        //         {
        //             permission_id = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             permission_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Permissions", x => x.permission_id);
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "Roles",
        //         columns: table => new
        //         {
        //             role_id = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             role_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Roles", x => x.role_id);
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "Users",
        //         columns: table => new
        //         {
        //             Id = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
        //             Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
        //             Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Users", x => x.Id);
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "Role_Permissions",
        //         columns: table => new
        //         {
        //             role_id = table.Column<int>(type: "int", nullable: false),
        //             permission_id = table.Column<int>(type: "int", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_Role_Permissions", x => new { x.role_id, x.permission_id });
        //             table.ForeignKey(
        //                 name: "FK_Role_Permissions_Permissions_permission_id",
        //                 column: x => x.permission_id,
        //                 principalTable: "Permissions",
        //                 principalColumn: "permission_id",
        //                 onDelete: ReferentialAction.Restrict);
        //             table.ForeignKey(
        //                 name: "FK_Role_Permissions_Roles_role_id",
        //                 column: x => x.role_id,
        //                 principalTable: "Roles",
        //                 principalColumn: "role_id",
        //                 onDelete: ReferentialAction.Restrict);
        //         });

        //     migrationBuilder.CreateTable(
        //         name: "UserCourses",
        //         columns: table => new
        //         {
        //             Id = table.Column<int>(type: "int", nullable: false)
        //                 .Annotation("SqlServer:Identity", "1, 1"),
        //             UserId = table.Column<int>(type: "int", nullable: false),
        //             CourseId = table.Column<int>(type: "int", nullable: false),
        //             EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
        //             Status = table.Column<int>(type: "int", nullable: false)
        //         },
        //         constraints: table =>
        //         {
        //             table.PrimaryKey("PK_UserCourses", x => x.Id);
        //             table.ForeignKey(
        //                 name: "FK_UserCourses_Course_CourseId",
        //                 column: x => x.CourseId,
        //                 principalTable: "Course",
        //                 principalColumn: "Id",
        //                 onDelete: ReferentialAction.Restrict);
        //             table.ForeignKey(
        //                 name: "FK_UserCourses_Users_UserId",
        //                 column: x => x.UserId,
        //                 principalTable: "Users",
        //                 principalColumn: "Id",
        //                 onDelete: ReferentialAction.Restrict);
        //         });

        //     migrationBuilder.CreateIndex(
        //         name: "IX_Role_Permissions_permission_id",
        //         table: "Role_Permissions",
        //         column: "permission_id");

        //     migrationBuilder.CreateIndex(
        //         name: "IX_UserCourses_CourseId",
        //         table: "UserCourses",
        //         column: "CourseId");

        //     migrationBuilder.CreateIndex(
        //         name: "IX_UserCourses_UserId",
        //         table: "UserCourses",
        //         column: "UserId");

        //     migrationBuilder.CreateIndex(
        //         name: "IX_Users_Username",
        //         table: "Users",
        //         column: "Username",
        //         unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "Role_Permissions");

            // migrationBuilder.DropTable(
            //     name: "UserCourses");

            // migrationBuilder.DropTable(
            //     name: "Permissions");

            // migrationBuilder.DropTable(
            //     name: "Roles");

            migrationBuilder.DropTable(
                name: "Course");

            // migrationBuilder.DropTable(
            //     name: "Users");
        }
    }
}
