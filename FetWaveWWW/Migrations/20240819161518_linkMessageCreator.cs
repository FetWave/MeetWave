using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FetWaveWWW.Migrations
{
    /// <inheritdoc />
    public partial class linkMessageCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "MessageThreads",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThreads_CreatedUserId",
                table: "MessageThreads",
                column: "CreatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageThreads_AspNetUsers_CreatedUserId",
                table: "MessageThreads",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageThreads_AspNetUsers_CreatedUserId",
                table: "MessageThreads");

            migrationBuilder.DropIndex(
                name: "IX_MessageThreads_CreatedUserId",
                table: "MessageThreads");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedUserId",
                table: "MessageThreads",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
