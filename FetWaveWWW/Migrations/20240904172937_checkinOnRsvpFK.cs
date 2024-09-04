using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class checkinOnRsvpFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CheckInUserId",
                table: "RSVPs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_CheckInUserId",
                table: "RSVPs",
                column: "CheckInUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSVPs_AspNetUsers_CheckInUserId",
                table: "RSVPs",
                column: "CheckInUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RSVPs_AspNetUsers_CheckInUserId",
                table: "RSVPs");

            migrationBuilder.DropIndex(
                name: "IX_RSVPs_CheckInUserId",
                table: "RSVPs");

            migrationBuilder.AlterColumn<string>(
                name: "CheckInUserId",
                table: "RSVPs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
