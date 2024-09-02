using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class feeToOrderOnReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Orders_FeeId",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "FeeId",
                table: "Receipts",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Receipts_FeeId",
                table: "Receipts",
                newName: "IX_Receipts_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Orders_OrderId",
                table: "Receipts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Orders_OrderId",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Receipts",
                newName: "FeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Receipts_OrderId",
                table: "Receipts",
                newName: "IX_Receipts_FeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Orders_FeeId",
                table: "Receipts",
                column: "FeeId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
