using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class messagingDTOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageThreads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageThreads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StringId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LineText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageLines_AspNetUsers_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageLines_MessageThreads_StringId",
                        column: x => x.StringId,
                        principalTable: "MessageThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageRecipients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThreadId = table.Column<long>(type: "bigint", nullable: false),
                    AddedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RemovedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemovedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RecipientUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageRecipients_AspNetUsers_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageRecipients_AspNetUsers_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MessageRecipients_AspNetUsers_RemovedByUserId",
                        column: x => x.RemovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_MessageRecipients_MessageThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "MessageThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "MessageReads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageRecipientId = table.Column<long>(type: "bigint", nullable: false),
                    MessageLineId = table.Column<long>(type: "bigint", nullable: false),
                    ReadTS = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReads_MessageLines_MessageLineId",
                        column: x => x.MessageLineId,
                        principalTable: "MessageLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReads_MessageRecipients_MessageRecipientId",
                        column: x => x.MessageRecipientId,
                        principalTable: "MessageRecipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLines_CreatedUserId",
                table: "MessageLines",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLines_StringId",
                table: "MessageLines",
                column: "StringId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageLineId",
                table: "MessageReads",
                column: "MessageLineId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageRecipientId",
                table: "MessageReads",
                column: "MessageRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_AddedByUserId",
                table: "MessageRecipients",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_RecipientUserId",
                table: "MessageRecipients",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_RemovedByUserId",
                table: "MessageRecipients",
                column: "RemovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipients_ThreadId",
                table: "MessageRecipients",
                column: "ThreadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageReads");

            migrationBuilder.DropTable(
                name: "MessageLines");

            migrationBuilder.DropTable(
                name: "MessageRecipients");

            migrationBuilder.DropTable(
                name: "MessageThreads");
        }
    }
}
