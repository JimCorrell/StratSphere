using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    [DbContext(typeof(StratSphereDbContext))]
    [Migration("20260313173449_AddActiveRosterSlotPartialUniqueIndex")]
    /// <inheritdoc />
    public partial class AddActiveRosterSlotPartialUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots");

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_active_card_season",
                table: "roster_slots",
                columns: new[] { "SeasonId", "CardId" },
                unique: true,
                filter: "\"DroppedAt\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roster_slots_active_card_season",
                table: "roster_slots");

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots",
                columns: new[] { "SeasonId", "CardId" });
        }
    }
}
