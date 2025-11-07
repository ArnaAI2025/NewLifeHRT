using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchMessageRecipients_Patients_PatientId",
                table: "BatchMessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_BatchMessages_AspNetUsers_ApprovedByUserId",
                table: "BatchMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Leads_LeadId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Patients_PatientId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "BatchMessages");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "BatchMessages");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUserId",
                table: "BatchMessages",
                newName: "StatusChangedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BatchMessages_ApprovedByUserId",
                table: "BatchMessages",
                newName: "IX_BatchMessages_StatusChangedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MessagesContent",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "Messages",
                type: "bit",
                maxLength: 20,
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direction",
                table: "Messages",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "BatchMessages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BatchMessages",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "BatchMessages",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BatchMessageRecipients",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorReason",
                table: "BatchMessageRecipients",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LeadId",
                table: "BatchMessageRecipients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BatchMessageRecipients_LeadId",
                table: "BatchMessageRecipients",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchMessageRecipients_Leads_LeadId",
                table: "BatchMessageRecipients",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BatchMessageRecipients_Patients_PatientId",
                table: "BatchMessageRecipients",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BatchMessages_AspNetUsers_StatusChangedByUserId",
                table: "BatchMessages",
                column: "StatusChangedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Leads_LeadId",
                table: "Conversations",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Patients_PatientId",
                table: "Conversations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BatchMessageRecipients_Leads_LeadId",
                table: "BatchMessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_BatchMessageRecipients_Patients_PatientId",
                table: "BatchMessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_BatchMessages_AspNetUsers_StatusChangedByUserId",
                table: "BatchMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Leads_LeadId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Patients_PatientId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_BatchMessageRecipients_LeadId",
                table: "BatchMessageRecipients");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "BatchMessageRecipients");

            migrationBuilder.RenameColumn(
                name: "StatusChangedByUserId",
                table: "BatchMessages",
                newName: "ApprovedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BatchMessages_StatusChangedByUserId",
                table: "BatchMessages",
                newName: "IX_BatchMessages_ApprovedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "MessagesContent",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "Messages",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 20,
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Direction",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "BatchMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BatchMessages",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "BatchMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "BatchMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "BatchMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BatchMessageRecipients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorReason",
                table: "BatchMessageRecipients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BatchMessageRecipients_Patients_PatientId",
                table: "BatchMessageRecipients",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BatchMessages_AspNetUsers_ApprovedByUserId",
                table: "BatchMessages",
                column: "ApprovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Leads_LeadId",
                table: "Conversations",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Patients_PatientId",
                table: "Conversations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
