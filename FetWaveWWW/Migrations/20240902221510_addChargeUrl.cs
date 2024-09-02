using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class addChargeUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventFees_AspNetUsers_CreatedUserId",
                table: "EventFees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventFees_Events_EventId",
                table: "EventFees");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLineItem_EventFees_OrderId",
                table: "OrderLineItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_EventFees_FeeId",
                table: "Receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventFees",
                table: "EventFees");

            migrationBuilder.RenameTable(
                name: "EventFees",
                newName: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_EventFees_EventId",
                table: "Orders",
                newName: "IX_Orders_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventFees_CreatedUserId",
                table: "Orders",
                newName: "IX_Orders_CreatedUserId");

            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLineItem_Orders_OrderId",
                table: "OrderLineItem",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedUserId",
                table: "Orders",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Orders_FeeId",
                table: "Receipts",
                column: "FeeId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction,
                onUpdate: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLineItem_Orders_OrderId",
                table: "OrderLineItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedUserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Orders_FeeId",
                table: "Receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "EventFees");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_EventId",
                table: "EventFees",
                newName: "IX_EventFees_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CreatedUserId",
                table: "EventFees",
                newName: "IX_EventFees_CreatedUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventFees",
                table: "EventFees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventFees_AspNetUsers_CreatedUserId",
                table: "EventFees",
                column: "CreatedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventFees_Events_EventId",
                table: "EventFees",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLineItem_EventFees_OrderId",
                table: "OrderLineItem",
                column: "OrderId",
                principalTable: "EventFees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_EventFees_FeeId",
                table: "Receipts",
                column: "FeeId",
                principalTable: "EventFees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
