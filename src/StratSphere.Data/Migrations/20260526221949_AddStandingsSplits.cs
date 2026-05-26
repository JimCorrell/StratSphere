using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStandingsSplits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AwayLosses",
                table: "standings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwayWins",
                table: "standings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeLosses",
                table: "standings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HomeWins",
                table: "standings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Last10Losses",
                table: "standings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Last10Wins",
                table: "standings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayLosses",
                table: "standings");

            migrationBuilder.DropColumn(
                name: "AwayWins",
                table: "standings");

            migrationBuilder.DropColumn(
                name: "HomeLosses",
                table: "standings");

            migrationBuilder.DropColumn(
                name: "HomeWins",
                table: "standings");

            migrationBuilder.DropColumn(
                name: "Last10Losses",
                table: "standings");

            migrationBuilder.DropColumn(
                name: "Last10Wins",
                table: "standings");
        }
    }
}
