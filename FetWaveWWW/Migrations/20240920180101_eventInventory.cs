using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class eventInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ItemPriceCents",
                table: "OrderLineItem",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "OrderLineItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "OrderLineItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    CreatedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemPriceCents = table.Column<long>(type: "bigint", nullable: false),
                    ItemAvailableCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineItem_InventoryId",
                table: "OrderLineItem",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_EventId",
                table: "Inventory",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLineItem_Inventory_InventoryId",
                table: "OrderLineItem",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLineItem_Inventory_InventoryId",
                table: "OrderLineItem");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_OrderLineItem_InventoryId",
                table: "OrderLineItem");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "OrderLineItem");

            migrationBuilder.AlterColumn<long>(
                name: "ItemPriceCents",
                table: "OrderLineItem",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "OrderLineItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
