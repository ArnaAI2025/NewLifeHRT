using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCountryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyOrderTracking_Orders_OrderId",
                table: "PharmacyOrderTracking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyOrderTracking",
                table: "PharmacyOrderTracking");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "PharmacyOrderTracking",
                newName: "PharmacyOrderTrackings");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacyOrderTracking_OrderId",
                table: "PharmacyOrderTrackings",
                newName: "IX_PharmacyOrderTrackings_OrderId");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId1",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "PharmacyOrderTrackings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourierServiceName",
                table: "PharmacyOrderTrackings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyOrderTrackings",
                table: "PharmacyOrderTrackings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryId1",
                table: "Addresses",
                column: "CountryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryId1",
                table: "Addresses",
                column: "CountryId1",
                principalTable: "Countries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyOrderTrackings_Orders_OrderId",
                table: "PharmacyOrderTrackings",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            SeedData.Clinic.InsertCountryData(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryId1",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyOrderTrackings_Orders_OrderId",
                table: "PharmacyOrderTrackings");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CountryId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CountryId1",
                table: "Addresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PharmacyOrderTrackings",
                table: "PharmacyOrderTrackings");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CountryId1",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "PharmacyOrderTrackings",
                newName: "PharmacyOrderTracking");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacyOrderTrackings_OrderId",
                table: "PharmacyOrderTracking",
                newName: "IX_PharmacyOrderTracking_OrderId");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Addresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "PharmacyOrderTracking",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CourierServiceName",
                table: "PharmacyOrderTracking",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PharmacyOrderTracking",
                table: "PharmacyOrderTracking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyOrderTracking_Orders_OrderId",
                table: "PharmacyOrderTracking",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
