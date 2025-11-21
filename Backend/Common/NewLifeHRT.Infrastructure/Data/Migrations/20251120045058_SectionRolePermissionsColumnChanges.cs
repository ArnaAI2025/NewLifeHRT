using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SectionRolePermissionsColumnChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModuleName",
                table: "Sections",
                type: "nvarchar(max)",
                nullable: true);

            // Fix: Change RolePermissions.Id (int identity → Guid)

            migrationBuilder.DropPrimaryKey("PK_RolePermissions", "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RolePermissions");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            // Fix: Change ActionTypes.EnumValue (int → string)

            migrationBuilder.DropColumn(
                name: "EnumValue",
                table: "ActionTypes");

            migrationBuilder.AddColumn<string>(
                name: "EnumValue",
                table: "ActionTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModuleName",
                table: "Sections");

            // Reverse RolePermissions.Id change
            migrationBuilder.DropPrimaryKey("PK_RolePermissions", "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RolePermissions");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RolePermissions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            // Reverse ActionTypes.EnumValue change
            migrationBuilder.DropColumn(
                name: "EnumValue",
                table: "ActionTypes");

            migrationBuilder.AddColumn<int>(
                name: "EnumValue",
                table: "ActionTypes",
                type: "int",
                maxLength: 250,
                nullable: false,
                defaultValue: 0);
        }
    }

}
