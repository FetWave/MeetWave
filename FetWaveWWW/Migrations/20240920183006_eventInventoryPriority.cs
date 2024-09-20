using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class eventInventoryPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Inventory",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Inventory");
        }
    }
}
