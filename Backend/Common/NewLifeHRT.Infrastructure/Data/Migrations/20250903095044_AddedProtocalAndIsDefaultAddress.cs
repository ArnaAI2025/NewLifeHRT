using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedProtocalAndIsDefaultAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultAddress",
                table: "Addresses");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultAddress",
                table: "ShippingAddresses",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "PharmacyOrderTracking",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "PharmacyOrderNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Protocol",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultAddress",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "Protocol",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "TrackingNumber",
                table: "PharmacyOrderTracking",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyOrderNumber",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultAddress",
                table: "Addresses",
                type: "bit",
                nullable: true);
        }
    }
}
