using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryKeyToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "code",
                table: "Course",
                newName: "Code");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Student",
                type: "int",
                nullable: true);

            // migrationBuilder.AddColumn<string>(
            //     name: "Description",
            //     table: "Course",
            //     type: "nvarchar(max)",
            //     nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_CourseId",
                table: "Student",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Course_CourseId",
                table: "Student",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Course_CourseId",
                table: "Student");

            migrationBuilder.DropIndex(
                name: "IX_Student_CourseId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Course",
                newName: "code");
        }
    }
}
