using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StratSphere.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lahman");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "batting",
                schema: "lahman",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    year_id = table.Column<int>(type: "integer", nullable: false),
                    stint = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<string>(type: "text", nullable: true),
                    lg_id = table.Column<string>(type: "text", nullable: true),
                    G = table.Column<int>(type: "integer", nullable: true),
                    AB = table.Column<int>(type: "integer", nullable: true),
                    R = table.Column<int>(type: "integer", nullable: true),
                    H = table.Column<int>(type: "integer", nullable: true),
                    doubles = table.Column<int>(type: "integer", nullable: true),
                    Triples = table.Column<int>(type: "integer", nullable: true),
                    HR = table.Column<int>(type: "integer", nullable: true),
                    RBI = table.Column<int>(type: "integer", nullable: true),
                    SB = table.Column<int>(type: "integer", nullable: true),
                    CS = table.Column<int>(type: "integer", nullable: true),
                    BB = table.Column<int>(type: "integer", nullable: true),
                    SO = table.Column<int>(type: "integer", nullable: true),
                    IBB = table.Column<int>(type: "integer", nullable: true),
                    HBP = table.Column<int>(type: "integer", nullable: true),
                    SH = table.Column<int>(type: "integer", nullable: true),
                    SF = table.Column<int>(type: "integer", nullable: true),
                    g_idp = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batting", x => new { x.player_id, x.year_id, x.stint });
                });

            migrationBuilder.CreateTable(
                name: "fielding",
                schema: "lahman",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    year_id = table.Column<int>(type: "integer", nullable: false),
                    stint = table.Column<int>(type: "integer", nullable: false),
                    pos = table.Column<string>(type: "text", nullable: false),
                    team_id = table.Column<string>(type: "text", nullable: true),
                    lg_id = table.Column<string>(type: "text", nullable: true),
                    G = table.Column<int>(type: "integer", nullable: true),
                    GS = table.Column<int>(type: "integer", nullable: true),
                    inn_outs = table.Column<int>(type: "integer", nullable: true),
                    PO = table.Column<int>(type: "integer", nullable: true),
                    A = table.Column<int>(type: "integer", nullable: true),
                    E = table.Column<int>(type: "integer", nullable: true),
                    DP = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fielding", x => new { x.player_id, x.year_id, x.stint, x.pos });
                });

            migrationBuilder.CreateTable(
                name: "people",
                schema: "lahman",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    birth_year = table.Column<int>(type: "integer", nullable: true),
                    birth_month = table.Column<int>(type: "integer", nullable: true),
                    birth_day = table.Column<int>(type: "integer", nullable: true),
                    name_first = table.Column<string>(type: "text", nullable: true),
                    name_last = table.Column<string>(type: "text", nullable: true),
                    name_given = table.Column<string>(type: "text", nullable: true),
                    bats = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    throws = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    debut = table.Column<DateOnly>(type: "date", nullable: true),
                    final_game = table.Column<DateOnly>(type: "date", nullable: true),
                    bbref_id = table.Column<string>(type: "text", nullable: true),
                    retro_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_people", x => x.player_id);
                });

            migrationBuilder.CreateTable(
                name: "pitching",
                schema: "lahman",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    year_id = table.Column<int>(type: "integer", nullable: false),
                    stint = table.Column<int>(type: "integer", nullable: false),
                    team_id = table.Column<string>(type: "text", nullable: true),
                    lg_id = table.Column<string>(type: "text", nullable: true),
                    W = table.Column<int>(type: "integer", nullable: true),
                    L = table.Column<int>(type: "integer", nullable: true),
                    G = table.Column<int>(type: "integer", nullable: true),
                    GS = table.Column<int>(type: "integer", nullable: true),
                    CG = table.Column<int>(type: "integer", nullable: true),
                    SHO = table.Column<int>(type: "integer", nullable: true),
                    SV = table.Column<int>(type: "integer", nullable: true),
                    ip_outs = table.Column<int>(type: "integer", nullable: true),
                    H = table.Column<int>(type: "integer", nullable: true),
                    ER = table.Column<int>(type: "integer", nullable: true),
                    HR = table.Column<int>(type: "integer", nullable: true),
                    BB = table.Column<int>(type: "integer", nullable: true),
                    SO = table.Column<int>(type: "integer", nullable: true),
                    era = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pitching", x => new { x.player_id, x.year_id, x.stint });
                });

            migrationBuilder.CreateTable(
                name: "player_cards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LahmanPlayerId = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    CardYear = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    CommissionerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "setup"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leagues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_leagues_AspNetUsers_CommissionerId",
                        column: x => x.CommissionerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "league_members",
                columns: table => new
                {
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_members", x => new { x.LeagueId, x.UserId });
                    table.ForeignKey(
                        name: "FK_league_members_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_league_members_leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CardYear = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seasons_leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_teams_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teams_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    GameDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_games_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_games_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_games_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roster_slots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roster_slots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roster_slots_player_cards_CardId",
                        column: x => x.CardId,
                        principalTable: "player_cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roster_slots_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_roster_slots_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sim_batting_stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    G = table.Column<int>(type: "integer", nullable: false),
                    AB = table.Column<int>(type: "integer", nullable: false),
                    R = table.Column<int>(type: "integer", nullable: false),
                    H = table.Column<int>(type: "integer", nullable: false),
                    Doubles = table.Column<int>(type: "integer", nullable: false),
                    Triples = table.Column<int>(type: "integer", nullable: false),
                    HR = table.Column<int>(type: "integer", nullable: false),
                    RBI = table.Column<int>(type: "integer", nullable: false),
                    BB = table.Column<int>(type: "integer", nullable: false),
                    SO = table.Column<int>(type: "integer", nullable: false),
                    SB = table.Column<int>(type: "integer", nullable: false),
                    CS = table.Column<int>(type: "integer", nullable: false),
                    HBP = table.Column<int>(type: "integer", nullable: false),
                    SF = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sim_batting_stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sim_batting_stats_player_cards_CardId",
                        column: x => x.CardId,
                        principalTable: "player_cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sim_batting_stats_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sim_batting_stats_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sim_pitching_stats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    W = table.Column<int>(type: "integer", nullable: false),
                    L = table.Column<int>(type: "integer", nullable: false),
                    G = table.Column<int>(type: "integer", nullable: false),
                    GS = table.Column<int>(type: "integer", nullable: false),
                    SV = table.Column<int>(type: "integer", nullable: false),
                    IPOuts = table.Column<int>(type: "integer", nullable: false),
                    H = table.Column<int>(type: "integer", nullable: false),
                    ER = table.Column<int>(type: "integer", nullable: false),
                    HR = table.Column<int>(type: "integer", nullable: false),
                    BB = table.Column<int>(type: "integer", nullable: false),
                    SO = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sim_pitching_stats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sim_pitching_stats_player_cards_CardId",
                        column: x => x.CardId,
                        principalTable: "player_cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sim_pitching_stats_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sim_pitching_stats_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "standings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    Ties = table.Column<int>(type: "integer", nullable: false),
                    RunsScored = table.Column<int>(type: "integer", nullable: false),
                    RunsAllowed = table.Column<int>(type: "integer", nullable: false),
                    Streak = table.Column<string>(type: "text", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_standings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_standings_seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_standings_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_AwayTeamId",
                table: "games",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_games_HomeTeamId",
                table: "games",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_games_SeasonId",
                table: "games",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_league_members_UserId",
                table: "league_members",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_leagues_CommissionerId",
                table: "leagues",
                column: "CommissionerId");

            migrationBuilder.CreateIndex(
                name: "IX_leagues_Slug",
                table: "leagues",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_cards_LahmanPlayerId_CardYear_Position",
                table: "player_cards",
                columns: new[] { "LahmanPlayerId", "CardYear", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_CardId",
                table: "roster_slots",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_SeasonId_CardId",
                table: "roster_slots",
                columns: new[] { "SeasonId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roster_slots_TeamId",
                table: "roster_slots",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_seasons_LeagueId",
                table: "seasons",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_sim_batting_stats_CardId_SeasonId",
                table: "sim_batting_stats",
                columns: new[] { "CardId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sim_batting_stats_SeasonId",
                table: "sim_batting_stats",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_sim_batting_stats_TeamId",
                table: "sim_batting_stats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_sim_pitching_stats_CardId_SeasonId",
                table: "sim_pitching_stats",
                columns: new[] { "CardId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sim_pitching_stats_SeasonId",
                table: "sim_pitching_stats",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_sim_pitching_stats_TeamId",
                table: "sim_pitching_stats",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_standings_SeasonId",
                table: "standings",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_standings_TeamId_SeasonId",
                table: "standings",
                columns: new[] { "TeamId", "SeasonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_LeagueId",
                table: "teams",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_SeasonId",
                table: "teams",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_teams_UserId",
                table: "teams",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "batting",
                schema: "lahman");

            migrationBuilder.DropTable(
                name: "fielding",
                schema: "lahman");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "league_members");

            migrationBuilder.DropTable(
                name: "people",
                schema: "lahman");

            migrationBuilder.DropTable(
                name: "pitching",
                schema: "lahman");

            migrationBuilder.DropTable(
                name: "roster_slots");

            migrationBuilder.DropTable(
                name: "sim_batting_stats");

            migrationBuilder.DropTable(
                name: "sim_pitching_stats");

            migrationBuilder.DropTable(
                name: "standings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "player_cards");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "leagues");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
