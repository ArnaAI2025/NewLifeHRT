using Microsoft.EntityFrameworkCore.Migrations;
using NewLifeHRT.Infrastructure.Data.Seed;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProposalStatusInProgressToInReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            SeedData.Clinic.UpdateProposalStatusInProgressToInReview(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
