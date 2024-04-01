using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteManagement.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mig4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Residents_ReceiverId",
                table: "Message");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Residents_ReceiverId",
                table: "Message",
                column: "ReceiverId",
                principalTable: "Residents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Residents_ReceiverId",
                table: "Message");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Residents_ReceiverId",
                table: "Message",
                column: "ReceiverId",
                principalTable: "Residents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
