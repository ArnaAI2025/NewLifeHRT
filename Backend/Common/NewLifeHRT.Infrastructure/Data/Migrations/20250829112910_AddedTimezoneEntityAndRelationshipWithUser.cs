using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimezoneEntityAndRelationshipWithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimezoneId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StandardName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TimezoneId",
                table: "AspNetUsers",
                column: "TimezoneId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TimeZones_TimezoneId",
                table: "AspNetUsers",
                column: "TimezoneId",
                principalTable: "TimeZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            SeedData.Clinic.InsertTimeZones(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TimeZones_TimezoneId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TimezoneId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TimezoneId",
                table: "AspNetUsers");
        }
    }
}
