using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixMedicationTypeSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherMedicationType",
                table: "MedicalRecommendations",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.Sql("DELETE FROM MedicationTypes");
            SeedData.Clinic.InsertMedicationTypes(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherMedicationType",
                table: "MedicalRecommendations");
        }
    }
}
