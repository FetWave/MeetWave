using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetWave.Migrations
{
    /// <inheritdoc />
    public partial class profiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pronouns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsPublicTS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublicApproverUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedTS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeletedTS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pronouns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pronouns_AspNetUsers_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pronouns_AspNetUsers_DeletedUserId",
                        column: x => x.DeletedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pronouns_AspNetUsers_PublicApproverUserId",
                        column: x => x.PublicApproverUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PronounsId = table.Column<int>(type: "int", nullable: true),
                    DefaultRegionId = table.Column<int>(type: "int", nullable: true),
                    PrivateProfile = table.Column<bool>(type: "bit", nullable: false),
                    DefaultPrivateRsvp = table.Column<bool>(type: "bit", nullable: true),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Profiles_Pronouns_PronounsId",
                        column: x => x.PronounsId,
                        principalTable: "Pronouns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_Regions_DefaultRegionId",
                        column: x => x.DefaultRegionId,
                        principalTable: "Regions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_DefaultRegionId",
                table: "Profiles",
                column: "DefaultRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_PronounsId",
                table: "Profiles",
                column: "PronounsId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronouns_CreatedUserId",
                table: "Pronouns",
                column: "CreatedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronouns_DeletedUserId",
                table: "Pronouns",
                column: "DeletedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pronouns_PublicApproverUserId",
                table: "Pronouns",
                column: "PublicApproverUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Pronouns");
        }
    }
}
