using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    [DbContext(typeof(StratSphereDbContext))]
    [Migration("20260316160120_AddLeagueAbbreviation")]
    /// <inheritdoc />
    public partial class AddLeagueAbbreviation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "leagues",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            // Backfill existing leagues with a guaranteed-unique abbreviation (LG0001, LG0002, …).
            // Commissioners can update their abbreviation after migrating.
            migrationBuilder.Sql(@"
                WITH numbered AS (
                    SELECT ""Id"", ROW_NUMBER() OVER (ORDER BY ""CreatedAt"") AS rn
                    FROM leagues
                    WHERE ""Abbreviation"" = ''
                )
                UPDATE leagues l
                SET ""Abbreviation"" = 'LG' || LPAD(CAST(n.rn AS VARCHAR), 4, '0')
                FROM numbered n
                WHERE l.""Id"" = n.""Id"";
            ");

            migrationBuilder.CreateIndex(
                name: "IX_leagues_Abbreviation",
                table: "leagues",
                column: "Abbreviation",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_leagues_Abbreviation",
                table: "leagues");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "leagues");
        }
    }
}
