using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLifeHRT.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsFromWebhookPropeeryToOrderAPItracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFromWebhook",
                table: "OrderProcessingApiTrackings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromWebhook",
                table: "OrderProcessingApiTrackings");
        }
    }
}
