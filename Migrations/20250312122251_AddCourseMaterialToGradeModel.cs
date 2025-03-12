using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseMaterialToGradeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseMaterialId",
                table: "Grades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CourseMaterialId",
                table: "Grades",
                column: "CourseMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_CourseMaterials_CourseMaterialId",
                table: "Grades",
                column: "CourseMaterialId",
                principalTable: "CourseMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_CourseMaterials_CourseMaterialId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_CourseMaterialId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "CourseMaterialId",
                table: "Grades");
        }
    }
}
