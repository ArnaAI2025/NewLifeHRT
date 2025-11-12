using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetToCascadeDeleteOnDeleteOfApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseInformation_AspNetUsers_UserId",
                table: "LicenseInformation");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseInformation_AspNetUsers_UserId",
                table: "LicenseInformation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseInformation_AspNetUsers_UserId",
                table: "LicenseInformation");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseInformation_AspNetUsers_UserId",
                table: "LicenseInformation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
