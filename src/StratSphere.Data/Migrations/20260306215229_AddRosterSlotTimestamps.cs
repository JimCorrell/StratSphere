using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    [DbContext(typeof(StratSphereDbContext))]
    [Migration("20260306215229_AddRosterSlotTimestamps")]
    /// <inheritdoc />
    public partial class AddRosterSlotTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AcquiredAt",
                table: "roster_slots",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DroppedAt",
                table: "roster_slots",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcquiredAt",
                table: "roster_slots");

            migrationBuilder.DropColumn(
                name: "DroppedAt",
                table: "roster_slots");
        }
    }
}
