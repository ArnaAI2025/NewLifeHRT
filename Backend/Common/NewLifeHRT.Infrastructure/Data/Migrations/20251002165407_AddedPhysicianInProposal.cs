using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhysicianInProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhysicianId",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_PhysicianId",
                table: "Proposals",
                column: "PhysicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_PhysicianId",
                table: "Proposals",
                column: "PhysicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_PhysicianId",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_PhysicianId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "PhysicianId",
                table: "Proposals");
        }
    }
}
