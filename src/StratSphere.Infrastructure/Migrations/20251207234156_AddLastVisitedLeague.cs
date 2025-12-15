using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLastVisitedLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LastVisitedLeagueId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastVisitedLeagueId",
                table: "Users",
                column: "LastVisitedLeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Leagues_LastVisitedLeagueId",
                table: "Users",
                column: "LastVisitedLeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Leagues_LastVisitedLeagueId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_LastVisitedLeagueId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastVisitedLeagueId",
                table: "Users");
        }
    }
}
