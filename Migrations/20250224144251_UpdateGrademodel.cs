using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGrademodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Users_userId",
                table: "Grades");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Grades",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_userId",
                table: "Grades",
                newName: "IX_Grades_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "Grades",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Users_UserId",
                table: "Grades",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Users_UserId",
                table: "Grades");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Grades",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Grades_UserId",
                table: "Grades",
                newName: "IX_Grades_userId");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Users_userId",
                table: "Grades",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
