using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPharmacyCurrencyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacyCurrencies");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Pharmacies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_CurrencyId",
                table: "Pharmacies",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_Currencies_CurrencyId",
                table: "Pharmacies",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_Currencies_CurrencyId",
                table: "Pharmacies");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_CurrencyId",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Pharmacies");

            migrationBuilder.CreateTable(
                name: "PharmacyCurrencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    PharmacyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyCurrencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacyCurrencies_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PharmacyCurrencies_Pharmacies_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "Pharmacies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyCurrencies_CurrencyId",
                table: "PharmacyCurrencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyCurrencies_PharmacyId_CurrencyId",
                table: "PharmacyCurrencies",
                columns: new[] { "PharmacyId", "CurrencyId" },
                unique: true);
        }
    }
}
