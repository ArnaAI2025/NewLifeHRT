using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Domain.Entities.Hospital;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProposalEntitiesAndProposalDetailEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_AspNetUsers_CounselorId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_ProposalDetails_Products_ProductId1",
                table: "ProposalDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProposalDetails_ProductId1",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "DiscountOnDeliveryCharge",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "DiscountOnSurcharge",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "TotalDeliveryCharge",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "TotalDiscount",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "ProposalDetails");

            migrationBuilder.RenameColumn(
                name: "TotalSurcharge",
                table: "Proposals",
                newName: "CouponDiscount");

            migrationBuilder.RenameColumn(
                name: "CounselorId",
                table: "Coupons",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Coupons_CounselorId",
                table: "Coupons",
                newName: "IX_Coupons_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Proposals",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "Proposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAddressVerified",
                table: "Proposals",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PerUnitAmount",
                table: "ProposalDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultAddress",
                table: "Addresses",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_AssignedUserId",
                table: "Proposals",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_AspNetUsers_UserId",
                table: "Coupons",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_AssignedUserId",
                table: "Proposals",
                column: "AssignedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            SeedData.Clinic.InsertShippingMethodData(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_AspNetUsers_UserId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_AssignedUserId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_AssignedUserId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "IsAddressVerified",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "PerUnitAmount",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "IsDefaultAddress",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "CouponDiscount",
                table: "Proposals",
                newName: "TotalSurcharge");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Coupons",
                newName: "CounselorId");

            migrationBuilder.RenameIndex(
                name: "IX_Coupons_UserId",
                table: "Coupons",
                newName: "IX_Coupons_CounselorId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountOnDeliveryCharge",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountOnSurcharge",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDeliveryCharge",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalDiscount",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ProposalDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "ProposalDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "ProposalDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProposalDetails_ProductId1",
                table: "ProposalDetails",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_AspNetUsers_CounselorId",
                table: "Coupons",
                column: "CounselorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProposalDetails_Products_ProductId1",
                table: "ProposalDetails",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
