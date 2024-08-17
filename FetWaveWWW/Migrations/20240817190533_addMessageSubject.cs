using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FetWaveWWW.Migrations
{
    /// <inheritdoc />
    public partial class addMessageSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "MessageThreads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "MessageThreads");
        }
    }
}
