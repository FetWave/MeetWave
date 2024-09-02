using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class dressCodeCatsManyToManyViaBuilder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CalendarEventCategory",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    EventsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEventCategory", x => new { x.CategoriesId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_CalendarEventCategory_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarEventCategory_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalendarEventDressCode",
                columns: table => new
                {
                    DressCodesId = table.Column<int>(type: "int", nullable: false),
                    EventsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEventDressCode", x => new { x.DressCodesId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_CalendarEventDressCode_DressCodes_DressCodesId",
                        column: x => x.DressCodesId,
                        principalTable: "DressCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CalendarEventDressCode_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEventCategory_EventsId",
                table: "CalendarEventCategory",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEventDressCode_EventsId",
                table: "CalendarEventDressCode",
                column: "EventsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEventCategory");

            migrationBuilder.DropTable(
                name: "CalendarEventDressCode");

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
    }
}
