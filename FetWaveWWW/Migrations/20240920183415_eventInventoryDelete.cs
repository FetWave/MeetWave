using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class eventInventoryDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteTS",
                table: "Inventory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeleteUserId",
                table: "Inventory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteTS",
                table: "Inventory");

            migrationBuilder.DropColumn(
                name: "DeleteUserId",
                table: "Inventory");
        }
    }
}
