using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedProposalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_AssignedUserId",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "Proposals",
                newName: "StatusUpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_AssignedUserId",
                table: "Proposals",
                newName: "IX_Proposals_StatusUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_StatusUpdatedById",
                table: "Proposals",
                column: "StatusUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_StatusUpdatedById",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "StatusUpdatedById",
                table: "Proposals",
                newName: "AssignedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Proposals_StatusUpdatedById",
                table: "Proposals",
                newName: "IX_Proposals_AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_AssignedUserId",
                table: "Proposals",
                column: "AssignedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
