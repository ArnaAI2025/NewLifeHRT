using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertiesandSeedDatainOrderTableAndNewTableCourierService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourierServiceId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourierServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Abbreviation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierServices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CourierServiceId",
                table: "Orders",
                column: "CourierServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_CourierServices_CourierServiceId",
                table: "Orders",
                column: "CourierServiceId",
                principalTable: "CourierServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            SeedData.Clinic.InsertCourierServices(migrationBuilder);
            SeedData.Clinic.UpdateCourierServicesAndTrackingNumberInOrder(migrationBuilder);
            SeedData.Clinic.UpdateOrderNumberInOrders(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_CourierServices_CourierServiceId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CourierServices");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CourierServiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CourierServiceId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Orders");
        }
    }
}
