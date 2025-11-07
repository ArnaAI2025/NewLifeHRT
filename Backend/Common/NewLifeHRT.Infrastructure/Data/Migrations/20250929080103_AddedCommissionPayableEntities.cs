using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommissionPayableEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryChargeOverRidden",
                table: "Proposals",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceOverRidden",
                table: "ProposalDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentage",
                table: "Pharmacies",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasFixedCommission",
                table: "Pharmacies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CommissionGeneratedDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeliveryChargeOverRidden",
                table: "Orders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriceOverRidden",
                table: "OrderDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoolDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CounselorId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolDetails_AspNetUsers_CounselorId",
                        column: x => x.CounselorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PoolDetails_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommissionsPayables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoolDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionBaseAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SyringeCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinancialResult = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionCalculationDetails = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CtcCalculationDetails = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsMissingProductPrice = table.Column<bool>(type: "bit", nullable: true),
                    EntryType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionsPayables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionsPayables_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionsPayables_PoolDetails_PoolDetailId",
                        column: x => x.PoolDetailId,
                        principalTable: "PoolDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommissionsPayablesDetails",
                columns: table => new
                {
                    CommissionsPayableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CTC = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinancialResult = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionPercentage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommissionPayable = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionsPayablesDetails", x => new { x.CommissionsPayableId, x.OrderDetailId });
                    table.ForeignKey(
                        name: "FK_CommissionsPayablesDetails_CommissionsPayables_CommissionsPayableId",
                        column: x => x.CommissionsPayableId,
                        principalTable: "CommissionsPayables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommissionsPayablesDetails_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionsPayables_OrderId_PoolDetailId_EntryType",
                table: "CommissionsPayables",
                columns: new[] { "OrderId", "PoolDetailId", "EntryType" },
                unique: true,
                filter: "[EntryType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionsPayables_PoolDetailId",
                table: "CommissionsPayables",
                column: "PoolDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionsPayablesDetails_OrderDetailId",
                table: "CommissionsPayablesDetails",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolDetails_CounselorId",
                table: "PoolDetails",
                column: "CounselorId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolDetails_PoolId",
                table: "PoolDetails",
                column: "PoolId");

            SeedData.Clinic.InsertStringeProductTypesData(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommissionsPayablesDetails");

            migrationBuilder.DropTable(
                name: "CommissionsPayables");

            migrationBuilder.DropTable(
                name: "PoolDetails");

            migrationBuilder.DropTable(
                name: "Pools");

            migrationBuilder.DropColumn(
                name: "IsDeliveryChargeOverRidden",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "IsPriceOverRidden",
                table: "ProposalDetails");

            migrationBuilder.DropColumn(
                name: "CommissionPercentage",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "HasFixedCommission",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "CommissionGeneratedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeliveryChargeOverRidden",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsPriceOverRidden",
                table: "OrderDetails");
        }
    }
}
