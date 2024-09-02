using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class stringToThread : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageLines_MessageThreads_StringId",
                table: "MessageLines");

            migrationBuilder.RenameColumn(
                name: "StringId",
                table: "MessageLines",
                newName: "ThreadId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageLines_StringId",
                table: "MessageLines",
                newName: "IX_MessageLines_ThreadId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageLines_MessageThreads_ThreadId",
                table: "MessageLines",
                column: "ThreadId",
                principalTable: "MessageThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageLines_MessageThreads_ThreadId",
                table: "MessageLines");

            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "MessageLines",
                newName: "StringId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageLines_ThreadId",
                table: "MessageLines",
                newName: "IX_MessageLines_StringId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageLines_MessageThreads_StringId",
                table: "MessageLines",
                column: "StringId",
                principalTable: "MessageThreads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
