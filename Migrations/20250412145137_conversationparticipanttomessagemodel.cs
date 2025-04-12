using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementApp.MVC.Migrations
{
    /// <inheritdoc />
    public partial class conversationparticipanttomessagemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversationParticipantId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationParticipantId",
                table: "Messages",
                column: "ConversationParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ConversationParticipants_ConversationParticipantId",
                table: "Messages",
                column: "ConversationParticipantId",
                principalTable: "ConversationParticipants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ConversationParticipants_ConversationParticipantId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationParticipantId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ConversationParticipantId",
                table: "Messages");
        }
    }
}
