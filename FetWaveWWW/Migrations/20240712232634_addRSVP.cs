using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FetWaveWWW.Migrations
{
    /// <inheritdoc />
    public partial class addRSVP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Unique_Id",
                table: "Events",
                newName: "UniqueId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_Unique_Id",
                table: "Events",
                newName: "IX_Events_UniqueId");

            migrationBuilder.CreateTable(
                name: "RSVPStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSVPStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RSVPs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedTS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EventId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RSVPStateId = table.Column<int>(type: "int", nullable: true),
                    Private = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSVPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RSVPs_AspNetUsers_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RSVPs_AspNetUsers_UpdatedUserId",
                        column: x => x.UpdatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RSVPs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RSVPs_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RSVPs_RSVPStates_RSVPStateId",
                        column: x => x.RSVPStateId,
                        principalTable: "RSVPStates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_CreatedUserId",
                table: "RSVPs",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_EventId",
                table: "RSVPs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_RSVPStateId",
                table: "RSVPs",
                column: "RSVPStateId");

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_UpdatedUserId",
                table: "RSVPs",
                column: "UpdatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RSVPs_UserId",
                table: "RSVPs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RSVPs");

            migrationBuilder.DropTable(
                name: "RSVPStates");

            migrationBuilder.RenameColumn(
                name: "UniqueId",
                table: "Events",
                newName: "Unique_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Events_UniqueId",
                table: "Events",
                newName: "IX_Events_Unique_Id");
        }
    }
}
