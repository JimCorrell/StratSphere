using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameNumberAndImportLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameNumber",
                table: "games",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "som_import_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GamesImported = table.Column<int>(type: "integer", nullable: false),
                    BattersImported = table.Column<int>(type: "integer", nullable: false),
                    PitchersImported = table.Column<int>(type: "integer", nullable: false),
                    UnmatchedPlayers = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_som_import_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_som_import_logs_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_som_import_logs_SeasonId",
                table: "som_import_logs",
                column: "SeasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "som_import_logs");

            migrationBuilder.DropColumn(
                name: "GameNumber",
                table: "games");
        }
    }
}
