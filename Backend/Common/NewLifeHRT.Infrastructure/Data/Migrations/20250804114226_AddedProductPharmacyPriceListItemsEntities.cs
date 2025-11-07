using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductPharmacyPriceListItemsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LifeFileDrugForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeFileDrugForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifeFileQuantityUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeFileQuantityUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LifeFileScheduleCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeFileScheduleCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductPharmacyPriceListItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostOfProduct = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LifeFilePharmacyProductId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LifeFielForeignPmsId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LifeFileDrugFormId = table.Column<int>(type: "int", nullable: true),
                    LifeFileDrugName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LifeFileDrugStrength = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LifeFileQuantityUnitId = table.Column<int>(type: "int", nullable: true),
                    LifeFileScheduledCodeId = table.Column<int>(type: "int", nullable: true),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPharmacyPriceListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPharmacyPriceListItems_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPharmacyPriceListItems_LifeFileDrugForms_LifeFileDrugFormId",
                        column: x => x.LifeFileDrugFormId,
                        principalTable: "LifeFileDrugForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPharmacyPriceListItems_LifeFileQuantityUnits_LifeFileQuantityUnitId",
                        column: x => x.LifeFileQuantityUnitId,
                        principalTable: "LifeFileQuantityUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPharmacyPriceListItems_LifeFileScheduleCodes_LifeFileScheduledCodeId",
                        column: x => x.LifeFileScheduledCodeId,
                        principalTable: "LifeFileScheduleCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPharmacyPriceListItems_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductPharmacyPriceListItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharmacyPriceListItems_CurrencyId",
                table: "ProductPharmacyPriceListItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharmacyPriceListItems_LifeFileDrugFormId",
                table: "ProductPharmacyPriceListItems",
                column: "LifeFileDrugFormId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharmacyPriceListItems_LifeFileQuantityUnitId",
                table: "ProductPharmacyPriceListItems",
                column: "LifeFileQuantityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharmacyPriceListItems_LifeFileScheduledCodeId",
                table: "ProductPharmacyPriceListItems",
                column: "LifeFileScheduledCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharmacyPriceListItems_PharmacyId",
                table: "ProductPharmacyPriceListItems",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPharmacyPriceListItems_ProductId",
                table: "ProductPharmacyPriceListItems",
                column: "ProductId");

            SeedData.Clinic.InsertLifeFileDrugFormData(migrationBuilder);
            SeedData.Clinic.InsertLifeFileScheduleCodeData(migrationBuilder);
            SeedData.Clinic.InsertLifeFileQuantityUnitsData(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPharmacyPriceListItems");

            migrationBuilder.DropTable(
                name: "LifeFileDrugForms");

            migrationBuilder.DropTable(
                name: "LifeFileQuantityUnits");

            migrationBuilder.DropTable(
                name: "LifeFileScheduleCodes");
        }
    }
}
