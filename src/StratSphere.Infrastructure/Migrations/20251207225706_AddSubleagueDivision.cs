using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubleagueDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Division",
                table: "Teams",
                newName: "DivisionName");

            migrationBuilder.RenameColumn(
                name: "Division",
                table: "StandingsEntries",
                newName: "DivisionName");

            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "Teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubleagueId",
                table: "Teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DivisionId",
                table: "StandingsEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Subleagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subleagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subleagues_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SubleagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Divisions_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Divisions_Subleagues_SubleagueId",
                        column: x => x.SubleagueId,
                        principalTable: "Subleagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_DivisionId",
                table: "Teams",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_SubleagueId",
                table: "Teams",
                column: "SubleagueId");

            migrationBuilder.CreateIndex(
                name: "IX_StandingsEntries_DivisionId",
                table: "StandingsEntries",
                column: "DivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_LeagueId_SubleagueId_Name",
                table: "Divisions",
                columns: new[] { "LeagueId", "SubleagueId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_SubleagueId",
                table: "Divisions",
                column: "SubleagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Subleagues_LeagueId_Name",
                table: "Subleagues",
                columns: new[] { "LeagueId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StandingsEntries_Divisions_DivisionId",
                table: "StandingsEntries",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Divisions_DivisionId",
                table: "Teams",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Subleagues_SubleagueId",
                table: "Teams",
                column: "SubleagueId",
                principalTable: "Subleagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StandingsEntries_Divisions_DivisionId",
                table: "StandingsEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Divisions_DivisionId",
                table: "Teams");

            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Subleagues_SubleagueId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Subleagues");

            migrationBuilder.DropIndex(
                name: "IX_Teams_DivisionId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_SubleagueId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_StandingsEntries_DivisionId",
                table: "StandingsEntries");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "SubleagueId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "DivisionId",
                table: "StandingsEntries");

            migrationBuilder.RenameColumn(
                name: "DivisionName",
                table: "Teams",
                newName: "Division");

            migrationBuilder.RenameColumn(
                name: "DivisionName",
                table: "StandingsEntries",
                newName: "Division");
        }
    }
}
