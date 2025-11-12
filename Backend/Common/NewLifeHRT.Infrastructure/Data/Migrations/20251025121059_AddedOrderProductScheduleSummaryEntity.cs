using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedOrderProductScheduleSummaryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductSchedules_Orders_OrderId",
                table: "OrderProductSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductSchedules_Products_ProductId",
                table: "OrderProductSchedules");

            migrationBuilder.DropIndex(
                name: "IX_OrderProductSchedules_OrderId",
                table: "OrderProductSchedules");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "OrderProductSchedules");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderProductSchedules",
                newName: "OrderProductScheduleSummaryId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProductSchedules_ProductId",
                table: "OrderProductSchedules",
                newName: "IX_OrderProductSchedules_OrderProductScheduleSummaryId");

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "OrderProductSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderProductScheduleSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FrequencyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Days = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductScheduleSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProductScheduleSummaries_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductScheduleSummaries_OrderDetailId",
                table: "OrderProductScheduleSummaries",
                column: "OrderDetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductSchedules_OrderProductScheduleSummaries_OrderProductScheduleSummaryId",
                table: "OrderProductSchedules",
                column: "OrderProductScheduleSummaryId",
                principalTable: "OrderProductScheduleSummaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductSchedules_OrderProductScheduleSummaries_OrderProductScheduleSummaryId",
                table: "OrderProductSchedules");

            migrationBuilder.DropTable(
                name: "OrderProductScheduleSummaries");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "OrderProductSchedules");

            migrationBuilder.RenameColumn(
                name: "OrderProductScheduleSummaryId",
                table: "OrderProductSchedules",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProductSchedules_OrderProductScheduleSummaryId",
                table: "OrderProductSchedules",
                newName: "IX_OrderProductSchedules_ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "OrderProductSchedules",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductSchedules_OrderId",
                table: "OrderProductSchedules",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductSchedules_Orders_OrderId",
                table: "OrderProductSchedules",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductSchedules_Products_ProductId",
                table: "OrderProductSchedules",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
