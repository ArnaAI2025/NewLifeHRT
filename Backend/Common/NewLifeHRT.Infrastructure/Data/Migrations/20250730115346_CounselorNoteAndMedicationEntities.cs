using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CounselorNoteAndMedicationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CounselorNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsAdminMailSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDoctorMailSent = table.Column<bool>(type: "bit", nullable: false),
                    CounselorId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounselorNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CounselorNotes_AspNetUsers_CounselorId",
                        column: x => x.CounselorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CounselorNotes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowUpLabTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpLabTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MedicationTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    MedicationTypeId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PMHx = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PSHx = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FHx = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Suppliments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Medication = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SocialHistory = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Allergies = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HRT = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FollowUpLabTestId = table.Column<int>(type: "int", nullable: true),
                    Subjective = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Objective = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Assessment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SocialPoint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ApplicationUserId = table.Column<int>(type: "int", nullable: true),
                    PatientId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecommendations_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedicalRecommendations_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecommendations_FollowUpLabTests_FollowUpLabTestId",
                        column: x => x.FollowUpLabTestId,
                        principalTable: "FollowUpLabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecommendations_MedicationTypes_MedicationTypeId",
                        column: x => x.MedicationTypeId,
                        principalTable: "MedicationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecommendations_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecommendations_Patients_PatientId1",
                        column: x => x.PatientId1,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounselorNotes_CounselorId",
                table: "CounselorNotes",
                column: "CounselorId");

            migrationBuilder.CreateIndex(
                name: "IX_CounselorNotes_PatientId",
                table: "CounselorNotes",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecommendations_ApplicationUserId",
                table: "MedicalRecommendations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecommendations_DoctorId",
                table: "MedicalRecommendations",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecommendations_FollowUpLabTestId",
                table: "MedicalRecommendations",
                column: "FollowUpLabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecommendations_MedicationTypeId",
                table: "MedicalRecommendations",
                column: "MedicationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecommendations_PatientId",
                table: "MedicalRecommendations",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecommendations_PatientId1",
                table: "MedicalRecommendations",
                column: "PatientId1");
            SeedData.Clinic.InsertFollowUpLabTests(migrationBuilder);
            SeedData.Clinic.InsertMedicationTypes(migrationBuilder);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounselorNotes");

            migrationBuilder.DropTable(
                name: "MedicalRecommendations");

            migrationBuilder.DropTable(
                name: "FollowUpLabTests");

            migrationBuilder.DropTable(
                name: "MedicationTypes");
        }
    }
}
