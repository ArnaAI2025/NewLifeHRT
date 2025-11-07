using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedProductPharmacyPriceListItemId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyShippingMethodId",
                table: "Proposals",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductPharmacyPriceListItemId",
                table: "ProposalDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "PharmacyOrderTrackings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsLab",
                table: "Pharmacies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyShippingMethodId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductPharmacyPriceListItemId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProposalDetails_ProductPharmacyPriceListItemId",
                table: "ProposalDetails",
                column: "ProductPharmacyPriceListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductPharmacyPriceListItemId",
                table: "OrderDetails",
                column: "ProductPharmacyPriceListItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductPharmacyPriceListItems_ProductPharmacyPriceListItemId",
                table: "OrderDetails",
                column: "ProductPharmacyPriceListItemId",
                principalTable: "ProductPharmacyPriceListItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalDetails_ProductPharmacyPriceListItems_ProductPharmacyPriceListItemId",
                table: "ProposalDetails",
                column: "ProductPharmacyPriceListItemId",
                principalTable: "ProductPharmacyPriceListItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductPharmacyPriceListItems_ProductPharmacyPriceListItemId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalDetails_ProductPharmacyPriceListItems_ProductPharmacyPriceListItemId",
                table: "ProposalDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProposalDetails_ProductPharmacyPriceListItemId",
                table: "ProposalDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductPharmacyPriceListItemId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductPharmacyPriceListItemId",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "IsLab",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "ProductPharmacyPriceListItemId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyShippingMethodId",
                table: "Proposals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "PharmacyOrderTrackings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PharmacyShippingMethodId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
