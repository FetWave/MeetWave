using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class breakOutLineItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Receipts_FeeId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "EventFees");

            migrationBuilder.DropColumn(
                name: "ItemPriceCents",
                table: "EventFees");

            migrationBuilder.CreateTable(
                name: "OrderLineItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemPriceCents = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLineItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLineItem_EventFees_OrderId",
                        column: x => x.OrderId,
                        principalTable: "EventFees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction,
                        onUpdate: ReferentialAction.NoAction
                        );
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_FeeId",
                table: "Receipts",
                column: "FeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineItem_OrderId",
                table: "OrderLineItem",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLineItem");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_FeeId",
                table: "Receipts");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "EventFees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ItemPriceCents",
                table: "EventFees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_FeeId",
                table: "Receipts",
                column: "FeeId");
        }
    }
}
