using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeamSeasonId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_teams_seasons_SeasonId",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "IX_teams_SeasonId",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "teams");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SeasonId",
                table: "teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_SeasonId",
                table: "teams",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_teams_seasons_SeasonId",
                table: "teams",
                column: "SeasonId",
                principalTable: "seasons",
                principalColumn: "Id");
        }
    }
}
