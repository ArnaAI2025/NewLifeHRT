using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPharmacyConfigurationEntityAndAddedPharmacyConfigurationDataEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyConfigurations_IntegrationKeys_KeyId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyConfigurations_PharmacyId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "PharmacyConfigurations");

            migrationBuilder.RenameColumn(
                name: "KeyId",
                table: "PharmacyConfigurations",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacyConfigurations_KeyId",
                table: "PharmacyConfigurations",
                newName: "IX_PharmacyConfigurations_TypeId");

            migrationBuilder.AddColumn<int>(
                name: "IntegrationKeyId",
                table: "PharmacyConfigurations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PharmacyConfigurationDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PharmacyConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacyConfigurationDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacyConfigurationDatas_IntegrationKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "IntegrationKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PharmacyConfigurationDatas_PharmacyConfigurations_PharmacyConfigurationId",
                        column: x => x.PharmacyConfigurationId,
                        principalTable: "PharmacyConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyConfigurations_IntegrationKeyId",
                table: "PharmacyConfigurations",
                column: "IntegrationKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyConfigurations_PharmacyId",
                table: "PharmacyConfigurations",
                column: "PharmacyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyConfigurations_PharmacyId_TypeId",
                table: "PharmacyConfigurations",
                columns: new[] { "PharmacyId", "TypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyConfigurationDatas_KeyId",
                table: "PharmacyConfigurationDatas",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyConfigurationDatas_PharmacyConfigurationId_KeyId",
                table: "PharmacyConfigurationDatas",
                columns: new[] { "PharmacyConfigurationId", "KeyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyConfigurations_IntegrationKeys_IntegrationKeyId",
                table: "PharmacyConfigurations",
                column: "IntegrationKeyId",
                principalTable: "IntegrationKeys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyConfigurations_IntegrationTypes_TypeId",
                table: "PharmacyConfigurations",
                column: "TypeId",
                principalTable: "IntegrationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyConfigurations_IntegrationKeys_IntegrationKeyId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyConfigurations_IntegrationTypes_TypeId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropTable(
                name: "PharmacyConfigurationDatas");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyConfigurations_IntegrationKeyId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyConfigurations_PharmacyId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyConfigurations_PharmacyId_TypeId",
                table: "PharmacyConfigurations");

            migrationBuilder.DropColumn(
                name: "IntegrationKeyId",
                table: "PharmacyConfigurations");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "PharmacyConfigurations",
                newName: "KeyId");

            migrationBuilder.RenameIndex(
                name: "IX_PharmacyConfigurations_TypeId",
                table: "PharmacyConfigurations",
                newName: "IX_PharmacyConfigurations_KeyId");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "PharmacyConfigurations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyConfigurations_PharmacyId",
                table: "PharmacyConfigurations",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyConfigurations_IntegrationKeys_KeyId",
                table: "PharmacyConfigurations",
                column: "KeyId",
                principalTable: "IntegrationKeys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
