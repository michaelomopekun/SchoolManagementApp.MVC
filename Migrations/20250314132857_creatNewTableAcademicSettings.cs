using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    /// <inheritdoc />
    public partial class creatNewTableAcademicSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentSession = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentSemester = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicSettings");
        }
    }
}
