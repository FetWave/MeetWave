using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class AddRsvpApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "RSVPs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedTS",
                table: "RSVPs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_ApprovedByUserId",
                table: "RSVPs",
                column: "ApprovedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RSVPs_AspNetUsers_ApprovedByUserId",
                table: "RSVPs",
                column: "ApprovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RSVPs_AspNetUsers_ApprovedByUserId",
                table: "RSVPs");

            migrationBuilder.DropIndex(
                name: "IX_RSVPs_ApprovedByUserId",
                table: "RSVPs");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "RSVPs");

            migrationBuilder.DropColumn(
                name: "ApprovedTS",
                table: "RSVPs");
        }
    }
}
