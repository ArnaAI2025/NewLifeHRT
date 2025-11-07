using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedFlagTOStartCalculationRefillDateAndAddedOrderProductDetailEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReadyForRefillDateCalculation",
                table: "OrderDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderProductRefillDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPharmacyPriceListItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaysSupply = table.Column<int>(type: "int", nullable: true),
                    RefillDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DoseAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DoseUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FrequencyPerDay = table.Column<int>(type: "int", nullable: true),
                    FrequencyPerWeek = table.Column<int>(type: "int", nullable: true),
                    BottleSizeMl = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    VialCount = table.Column<int>(type: "int", nullable: true),
                    UnitsCount = table.Column<int>(type: "int", nullable: true),
                    ClicksPerBottle = table.Column<int>(type: "int", nullable: true),
                    Assumptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductRefillDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProductRefillDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProductRefillDetails_ProductPharmacyPriceListItems_ProductPharmacyPriceListItemId",
                        column: x => x.ProductPharmacyPriceListItemId,
                        principalTable: "ProductPharmacyPriceListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductRefillDetails_OrderId",
                table: "OrderProductRefillDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductRefillDetails_ProductPharmacyPriceListItemId",
                table: "OrderProductRefillDetails",
                column: "ProductPharmacyPriceListItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProductRefillDetails");

            migrationBuilder.DropColumn(
                name: "IsReadyForRefillDateCalculation",
                table: "OrderDetails");
        }
    }
}
