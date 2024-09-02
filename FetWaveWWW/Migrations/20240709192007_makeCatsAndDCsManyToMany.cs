using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class makeCatsAndDCsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_DressCodes_DressCodeId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_CategoryId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_DressCodeId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DressCodeId",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "CalendarEventId",
                table: "DressCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalendarEventId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DressCodes_CalendarEventId",
                table: "DressCodes",
                column: "CalendarEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CalendarEventId",
                table: "Categories",
                column: "CalendarEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Events_CalendarEventId",
                table: "Categories",
                column: "CalendarEventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DressCodes_Events_CalendarEventId",
                table: "DressCodes",
                column: "CalendarEventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Events_CalendarEventId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_DressCodes_Events_CalendarEventId",
                table: "DressCodes");

            migrationBuilder.DropIndex(
                name: "IX_DressCodes_CalendarEventId",
                table: "DressCodes");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CalendarEventId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "DressCodes");

            migrationBuilder.DropColumn(
                name: "CalendarEventId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DressCodeId",
                table: "Events",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CategoryId",
                table: "Events",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_DressCodeId",
                table: "Events",
                column: "DressCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_DressCodes_DressCodeId",
                table: "Events",
                column: "DressCodeId",
                principalTable: "DressCodes",
                principalColumn: "Id");
        }
    }
}
