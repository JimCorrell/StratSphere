using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StratSphere.Data.Migrations
{
    [DbContext(typeof(StratSphereDbContext))]
    [Migration("20260313175334_LeagueStatusEnum")]
    /// <inheritdoc />
    public partial class LeagueStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "leagues",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValueSql: "'Setup'",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "setup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "leagues",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "setup",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValueSql: "'Setup'");
        }
    }
}
