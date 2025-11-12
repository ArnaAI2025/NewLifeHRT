using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MultiRoleUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers");

            migrationBuilder.Sql(@"
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                SELECT Id, RoleId
                FROM AspNetUsers
                WHERE RoleId IS NOT NULL
                  AND NOT EXISTS (
                      SELECT 1 FROM AspNetUserRoles ur
                      WHERE ur.UserId = AspNetUsers.Id AND ur.RoleId = AspNetUsers.RoleId
                  );
            ");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE u
                SET RoleId = ur.RoleId
                FROM AspNetUsers u
                OUTER APPLY (
                    SELECT TOP 1 RoleId
                    FROM AspNetUserRoles
                    WHERE UserId = u.Id
                    ORDER BY RoleId
                ) ur
                WHERE ur.RoleId IS NOT NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RoleId",
                table: "AspNetUsers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
