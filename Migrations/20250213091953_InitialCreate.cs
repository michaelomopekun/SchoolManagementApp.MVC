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
            // migrationBuilder.CreateTable(
            //     name: "Course",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         Credit = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //         Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Course", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Lecturers",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //         LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //         Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Lecturers", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Student",
            //     columns: table => new
            //     {
            //         Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //         Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //         Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         CourseId = table.Column<int>(type: "int", nullable: true)
            //     },
                // constraints: table =>
                // {
                //     table.PrimaryKey("PK_Student", x => x.Id);
                //     table.ForeignKey(
                //         name: "FK_Student_Course_CourseId",
                //         column: x => x.CourseId,
                //         principalTable: "Course",
                //         principalColumn: "Id");
                // });

            // migrationBuilder.CreateIndex(
            //     name: "IX_Student_CourseId",
            //     table: "Student",
            //     column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lecturers");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "Course");
        }
    }
}
