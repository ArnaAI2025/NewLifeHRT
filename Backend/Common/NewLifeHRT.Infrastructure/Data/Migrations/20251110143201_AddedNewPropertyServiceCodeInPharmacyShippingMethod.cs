using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertyServiceCodeInPharmacyShippingMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceCode",
                table: "PharmacyShippingMethods",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceCode",
                table: "PharmacyShippingMethods");
        }
    }
}
