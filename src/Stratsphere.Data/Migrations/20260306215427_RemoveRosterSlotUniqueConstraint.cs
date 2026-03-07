using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRosterSlotUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots");

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots",
                columns: new[] { "SeasonId", "CardId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots");

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots",
                columns: new[] { "SeasonId", "CardId" },
                unique: true);
        }
    }
}
