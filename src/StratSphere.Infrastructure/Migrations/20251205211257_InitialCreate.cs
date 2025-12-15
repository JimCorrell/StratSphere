using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StratSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActiveRosterSize = table.Column<int>(type: "integer", nullable: false),
                    CurrentPhase = table.Column<int>(type: "integer", nullable: false),
                    CurrentSeason = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MaxTeams = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RosterSize = table.Column<int>(type: "integer", nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UseDH = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlbAllstars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GP = table.Column<string>(type: "text", nullable: true),
                    GameId = table.Column<string>(type: "text", nullable: true),
                    GameNum = table.Column<string>(type: "text", nullable: true),
                    LgId = table.Column<string>(type: "text", nullable: true),
                    PlayerId = table.Column<string>(type: "text", nullable: true),
                    StartingPos = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbAllstars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlbBattings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AB = table.Column<int>(type: "integer", nullable: true),
                    BA = table.Column<decimal>(type: "numeric", nullable: true),
                    BB = table.Column<int>(type: "integer", nullable: true),
                    CS = table.Column<int>(type: "integer", nullable: true),
                    Doubles = table.Column<int>(type: "integer", nullable: true),
                    GIDP = table.Column<int>(type: "integer", nullable: true),
                    Games = table.Column<int>(type: "integer", nullable: true),
                    HBP = table.Column<int>(type: "integer", nullable: true),
                    HR = table.Column<int>(type: "integer", nullable: true),
                    Hits = table.Column<int>(type: "integer", nullable: true),
                    IBB = table.Column<int>(type: "integer", nullable: true),
                    LgId = table.Column<string>(type: "text", nullable: true),
                    OBP = table.Column<decimal>(type: "numeric", nullable: true),
                    OPS = table.Column<decimal>(type: "numeric", nullable: true),
                    PlayerId = table.Column<string>(type: "text", nullable: true),
                    RBIs = table.Column<int>(type: "integer", nullable: true),
                    Runs = table.Column<int>(type: "integer", nullable: true),
                    SB = table.Column<int>(type: "integer", nullable: true),
                    SF = table.Column<int>(type: "integer", nullable: true),
                    SH = table.Column<int>(type: "integer", nullable: true),
                    SLG = table.Column<decimal>(type: "numeric", nullable: true),
                    SO = table.Column<int>(type: "integer", nullable: true),
                    Stint = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<string>(type: "text", nullable: true),
                    Triples = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbBattings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlbFieldings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    A = table.Column<int>(type: "integer", nullable: true),
                    DP = table.Column<int>(type: "integer", nullable: true),
                    E = table.Column<int>(type: "integer", nullable: true),
                    Fpct = table.Column<decimal>(type: "numeric", nullable: true),
                    GS = table.Column<int>(type: "integer", nullable: true),
                    Games = table.Column<int>(type: "integer", nullable: true),
                    InnOuts = table.Column<int>(type: "integer", nullable: true),
                    Lg = table.Column<int>(type: "integer", nullable: true),
                    PO = table.Column<int>(type: "integer", nullable: true),
                    PlayerId = table.Column<string>(type: "text", nullable: true),
                    Pos = table.Column<string>(type: "text", nullable: true),
                    Stint = table.Column<int>(type: "integer", nullable: false),
                    Tm = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbFieldings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlbHallOfFames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ballots = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    NeededVotes = table.Column<int>(type: "integer", nullable: true),
                    PlayerId = table.Column<string>(type: "text", nullable: true),
                    VotePct = table.Column<decimal>(type: "numeric", nullable: true),
                    Votes = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    YearElected = table.Column<int>(type: "integer", nullable: true),
                    YearOnBallot = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbHallOfFames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlbPeople",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Bats = table.Column<char>(type: "character(1)", nullable: true),
                    BbRefUrl = table.Column<string>(type: "text", nullable: true),
                    BirthCity = table.Column<string>(type: "text", nullable: true),
                    BirthCountry = table.Column<string>(type: "text", nullable: true),
                    BirthDay = table.Column<int>(type: "integer", nullable: true),
                    BirthMonth = table.Column<int>(type: "integer", nullable: true),
                    BirthState = table.Column<string>(type: "text", nullable: true),
                    BirthYear = table.Column<int>(type: "integer", nullable: true),
                    DeathCity = table.Column<string>(type: "text", nullable: true),
                    DeathCountry = table.Column<string>(type: "text", nullable: true),
                    DeathDay = table.Column<int>(type: "integer", nullable: true),
                    DeathMonth = table.Column<int>(type: "integer", nullable: true),
                    DeathState = table.Column<string>(type: "text", nullable: true),
                    DeathYear = table.Column<int>(type: "integer", nullable: true),
                    Debut = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinalGame = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    NameFirst = table.Column<string>(type: "text", nullable: true),
                    NameGiven = table.Column<string>(type: "text", nullable: true),
                    NameLast = table.Column<string>(type: "text", nullable: true),
                    PlayerId = table.Column<string>(type: "text", nullable: false),
                    RetroSheetId = table.Column<string>(type: "text", nullable: true),
                    Throws = table.Column<char>(type: "character(1)", nullable: true),
                    Weight = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbPeople", x => x.Id);
                    table.UniqueConstraint("AK_MlbPeople_PlayerId", x => x.PlayerId);
                });

            migrationBuilder.CreateTable(
                name: "MlbPitchings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AREA = table.Column<int>(type: "integer", nullable: true),
                    BB = table.Column<int>(type: "integer", nullable: true),
                    BK = table.Column<int>(type: "integer", nullable: true),
                    CG = table.Column<int>(type: "integer", nullable: true),
                    ER = table.Column<int>(type: "integer", nullable: true),
                    ERA = table.Column<decimal>(type: "numeric", nullable: true),
                    G = table.Column<int>(type: "integer", nullable: true),
                    GS = table.Column<int>(type: "integer", nullable: true),
                    H = table.Column<int>(type: "integer", nullable: true),
                    HBP = table.Column<int>(type: "integer", nullable: true),
                    HR = table.Column<int>(type: "integer", nullable: true),
                    IBB = table.Column<int>(type: "integer", nullable: true),
                    IPouts = table.Column<int>(type: "integer", nullable: true),
                    K = table.Column<int>(type: "integer", nullable: true),
                    L = table.Column<int>(type: "integer", nullable: true),
                    LgId = table.Column<string>(type: "text", nullable: true),
                    PlayerId = table.Column<string>(type: "text", nullable: true),
                    SHO = table.Column<int>(type: "integer", nullable: true),
                    SV = table.Column<int>(type: "integer", nullable: true),
                    Stint = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<string>(type: "text", nullable: true),
                    W = table.Column<int>(type: "integer", nullable: true),
                    WHIP = table.Column<decimal>(type: "numeric", nullable: true),
                    WP = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbPitchings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MlbTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtBats = table.Column<int>(type: "integer", nullable: true),
                    Attendance = table.Column<int>(type: "integer", nullable: true),
                    BA = table.Column<decimal>(type: "numeric", nullable: true),
                    BPF = table.Column<int>(type: "integer", nullable: true),
                    BbRef = table.Column<int>(type: "integer", nullable: true),
                    BestRank = table.Column<int>(type: "integer", nullable: true),
                    CaughtStealing = table.Column<int>(type: "integer", nullable: true),
                    DivId = table.Column<string>(type: "text", nullable: true),
                    Doubles = table.Column<int>(type: "integer", nullable: true),
                    E = table.Column<int>(type: "integer", nullable: true),
                    Errs = table.Column<int>(type: "integer", nullable: true),
                    Fpct = table.Column<decimal>(type: "numeric", nullable: true),
                    Games = table.Column<int>(type: "integer", nullable: true),
                    GamesAway = table.Column<int>(type: "integer", nullable: true),
                    GamesHome = table.Column<int>(type: "integer", nullable: true),
                    Hits = table.Column<int>(type: "integer", nullable: true),
                    HomeRuns = table.Column<int>(type: "integer", nullable: true),
                    League = table.Column<string>(type: "text", nullable: true),
                    Losses = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Name_park_ref = table.Column<int>(type: "integer", nullable: true),
                    OBP = table.Column<decimal>(type: "numeric", nullable: true),
                    OPS = table.Column<decimal>(type: "numeric", nullable: true),
                    PPF = table.Column<int>(type: "integer", nullable: true),
                    Park = table.Column<string>(type: "text", nullable: true),
                    Pitchers = table.Column<int>(type: "integer", nullable: true),
                    RankDiv = table.Column<int>(type: "integer", nullable: true),
                    RankLg = table.Column<int>(type: "integer", nullable: true),
                    Runs = table.Column<int>(type: "integer", nullable: true),
                    SLG = table.Column<decimal>(type: "numeric", nullable: true),
                    StolenBases = table.Column<int>(type: "integer", nullable: true),
                    StrikeOuts = table.Column<int>(type: "integer", nullable: true),
                    TeamBbRef = table.Column<string>(type: "text", nullable: true),
                    TeamId = table.Column<string>(type: "text", nullable: true),
                    Triples = table.Column<int>(type: "integer", nullable: true),
                    Wins = table.Column<int>(type: "integer", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlbTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatsHand = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    BirthCountry = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentMlbOrg = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrentMlbTeam = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    MlbId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    MlbamId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PrimaryPosition = table.Column<int>(type: "integer", nullable: false),
                    SecondaryPositions = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StratCardNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StratYear = table.Column<int>(type: "integer", nullable: true),
                    ThrowsHand = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllowTrading = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentPick = table.Column<int>(type: "integer", nullable: false),
                    CurrentPickDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentRound = table.Column<int>(type: "integer", nullable: false),
                    CurrentTeamOnClock = table.Column<Guid>(type: "uuid", nullable: true),
                    Mode = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PickTimeLimitSeconds = table.Column<int>(type: "integer", nullable: false),
                    ScheduledStartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SnakeDraft = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalRounds = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drafts_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCurrentSeason = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Phase = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seasons_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StratTeams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Division = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Owner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    RosterCount40Man = table.Column<int>(type: "integer", nullable: false),
                    SecondaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    TotalRosterCount = table.Column<int>(type: "integer", nullable: false),
                    TotalSalary = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StratTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StratTeams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AtBats = table.Column<int>(type: "integer", nullable: true),
                    BB9 = table.Column<decimal>(type: "numeric", nullable: true),
                    BattingAverage = table.Column<decimal>(type: "numeric", nullable: true),
                    Doubles = table.Column<int>(type: "integer", nullable: true),
                    ERA = table.Column<decimal>(type: "numeric", nullable: true),
                    EarnedRuns = table.Column<int>(type: "integer", nullable: true),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    Hits = table.Column<int>(type: "integer", nullable: true),
                    HitsAllowed = table.Column<int>(type: "integer", nullable: true),
                    HomeRuns = table.Column<int>(type: "integer", nullable: true),
                    InningsPitched = table.Column<decimal>(type: "numeric", nullable: true),
                    K9 = table.Column<decimal>(type: "numeric", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: true),
                    OPS = table.Column<decimal>(type: "numeric", nullable: true),
                    OnBasePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    PitchingStrikeouts = table.Column<int>(type: "integer", nullable: true),
                    PitchingWalks = table.Column<int>(type: "integer", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RBI = table.Column<int>(type: "integer", nullable: true),
                    Runs = table.Column<int>(type: "integer", nullable: true),
                    Saves = table.Column<int>(type: "integer", nullable: true),
                    Season = table.Column<int>(type: "integer", nullable: false),
                    SluggingPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    StolenBases = table.Column<int>(type: "integer", nullable: true),
                    Strikeouts = table.Column<int>(type: "integer", nullable: true),
                    TeamName = table.Column<string>(type: "text", nullable: true),
                    Triples = table.Column<int>(type: "integer", nullable: true),
                    WAR = table.Column<decimal>(type: "numeric", nullable: true),
                    WHIP = table.Column<decimal>(type: "numeric", nullable: true),
                    Walks = table.Column<int>(type: "integer", nullable: true),
                    Wins = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerStats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeagueMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoutingReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ArmTool = table.Column<int>(type: "integer", nullable: true),
                    ChangeupTool = table.Column<int>(type: "integer", nullable: true),
                    Comparable = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ControlTool = table.Column<int>(type: "integer", nullable: true),
                    CurveballTool = table.Column<int>(type: "integer", nullable: true),
                    ETA = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FastballTool = table.Column<int>(type: "integer", nullable: true),
                    FieldingTool = table.Column<int>(type: "integer", nullable: true),
                    HitTool = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OverallGrade = table.Column<int>(type: "integer", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    PotentialGrade = table.Column<int>(type: "integer", nullable: true),
                    PowerTool = table.Column<int>(type: "integer", nullable: true),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ScoutedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScoutedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SliderTool = table.Column<int>(type: "integer", nullable: true),
                    SpeedTool = table.Column<int>(type: "integer", nullable: true),
                    Strengths = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Weaknesses = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoutingReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScoutingReports_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoutingReports_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScoutingReports_Users_ScoutedByUserId",
                        column: x => x.ScoutedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Conference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Division = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Teams_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StratPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AtBats = table.Column<int>(type: "integer", nullable: true),
                    BaseSalary = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Bats = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    ContractCost = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentSeasonWar = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Decision = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GamesStarted = table.Column<int>(type: "integer", nullable: true),
                    InningsPitched = table.Column<decimal>(type: "numeric(6,1)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsOn40Man = table.Column<bool>(type: "boolean", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false),
                    MlbPlayerId = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Points = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    PreviousSeasonWar = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    PrimaryPosition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    QualifyingOffer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SignedInfo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StratTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    Throws = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StratPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StratPlayers_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StratPlayers_MlbPeople_MlbPlayerId",
                        column: x => x.MlbPlayerId,
                        principalTable: "MlbPeople",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_StratPlayers_StratTeams_StratTeamId",
                        column: x => x.StratTeamId,
                        principalTable: "StratTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DraftOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    PickNumber = table.Column<int>(type: "integer", nullable: false),
                    PositionInRound = table.Column<int>(type: "integer", nullable: false),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftOrders_Drafts_DraftId",
                        column: x => x.DraftId,
                        principalTable: "Drafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftOrders_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftOrders_Teams_OriginalTeamId",
                        column: x => x.OriginalTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DraftOrders_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DraftPicks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAutoPick = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    OverallPickNumber = table.Column<int>(type: "integer", nullable: false),
                    PickMadeAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Round = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftPicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DraftPicks_Drafts_DraftId",
                        column: x => x.DraftId,
                        principalTable: "Drafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftPicks_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftPicks_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DraftPicks_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    AwayTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GameNumber = table.Column<int>(type: "integer", nullable: true),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    HomeTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsComplete = table.Column<bool>(type: "boolean", nullable: false),
                    IsPlayoff = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PlayoffRound = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameResults_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameResults_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameResults_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameResults_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RosterEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AcquiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcquiredVia = table.Column<int>(type: "integer", nullable: false),
                    ContractSalary = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    ContractYearRemaining = table.Column<int>(type: "integer", nullable: true),
                    ContractYears = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RosterPosition = table.Column<int>(type: "integer", nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RosterEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RosterEntries_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RosterEntries_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RosterEntries_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandingsEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Division = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DivisionRank = table.Column<int>(type: "integer", nullable: true),
                    GamesBack = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: true),
                    Last10Losses = table.Column<int>(type: "integer", nullable: true),
                    Last10Wins = table.Column<int>(type: "integer", nullable: true),
                    LeagueRank = table.Column<int>(type: "integer", nullable: true),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    RunsAllowed = table.Column<int>(type: "integer", nullable: true),
                    RunsScored = table.Column<int>(type: "integer", nullable: true),
                    SeasonId = table.Column<Guid>(type: "uuid", nullable: false),
                    Streak = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ties = table.Column<int>(type: "integer", nullable: true),
                    WinningPercentage = table.Column<decimal>(type: "numeric(5,3)", precision: 5, scale: 3, nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandingsEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StandingsEntries_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandingsEntries_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandingsEntries_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DraftId = table.Column<Guid>(type: "uuid", nullable: true),
                    DraftPickNumber = table.Column<int>(type: "integer", nullable: true),
                    DraftRound = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    OtherTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeagueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Drafts_DraftId",
                        column: x => x.DraftId,
                        principalTable: "Drafts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Teams_OtherTeamId",
                        column: x => x.OtherTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DraftOrders_DraftId",
                table: "DraftOrders",
                column: "DraftId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftOrders_LeagueId",
                table: "DraftOrders",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftOrders_OriginalTeamId",
                table: "DraftOrders",
                column: "OriginalTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftOrders_TeamId",
                table: "DraftOrders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftPicks_DraftId_OverallPickNumber",
                table: "DraftPicks",
                columns: new[] { "DraftId", "OverallPickNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DraftPicks_LeagueId",
                table: "DraftPicks",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftPicks_PlayerId",
                table: "DraftPicks",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_DraftPicks_TeamId",
                table: "DraftPicks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Drafts_LeagueId",
                table: "Drafts",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_AwayTeamId",
                table: "GameResults",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_HomeTeamId",
                table: "GameResults",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_LeagueId",
                table: "GameResults",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_GameResults_SeasonId",
                table: "GameResults",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_LeagueId_UserId",
                table: "LeagueMembers",
                columns: new[] { "LeagueId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueMembers_UserId",
                table: "LeagueMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_Slug",
                table: "Leagues",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_LastName_FirstName",
                table: "Players",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Players_MlbamId",
                table: "Players",
                column: "MlbamId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_MlbId",
                table: "Players",
                column: "MlbId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_PlayerId",
                table: "PlayerStats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterEntries_LeagueId_PlayerId",
                table: "RosterEntries",
                columns: new[] { "LeagueId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RosterEntries_PlayerId",
                table: "RosterEntries",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_RosterEntries_TeamId",
                table: "RosterEntries",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoutingReports_LeagueId",
                table: "ScoutingReports",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoutingReports_PlayerId",
                table: "ScoutingReports",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoutingReports_ScoutedByUserId",
                table: "ScoutingReports",
                column: "ScoutedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_LeagueId_Year",
                table: "Seasons",
                columns: new[] { "LeagueId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandingsEntries_LeagueId",
                table: "StandingsEntries",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_StandingsEntries_SeasonId_TeamId",
                table: "StandingsEntries",
                columns: new[] { "SeasonId", "TeamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandingsEntries_TeamId",
                table: "StandingsEntries",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_StratPlayers_DisplayName",
                table: "StratPlayers",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_StratPlayers_IsActive",
                table: "StratPlayers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_StratPlayers_LeagueId",
                table: "StratPlayers",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_StratPlayers_MlbPlayerId",
                table: "StratPlayers",
                column: "MlbPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_StratPlayers_StratTeamId",
                table: "StratPlayers",
                column: "StratTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_StratTeams_Abbreviation",
                table: "StratTeams",
                column: "Abbreviation");

            migrationBuilder.CreateIndex(
                name: "IX_StratTeams_IsActive",
                table: "StratTeams",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_StratTeams_LeagueId",
                table: "StratTeams",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_StratTeams_LeagueId_Abbreviation",
                table: "StratTeams",
                columns: new[] { "LeagueId", "Abbreviation" },
                unique: true,
                filter: "\"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId_Abbreviation",
                table: "Teams",
                columns: new[] { "LeagueId", "Abbreviation" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId_Name",
                table: "Teams",
                columns: new[] { "LeagueId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_OwnerId",
                table: "Teams",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DraftId",
                table: "Transactions",
                column: "DraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LeagueId",
                table: "Transactions",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OtherTeamId",
                table: "Transactions",
                column: "OtherTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PlayerId",
                table: "Transactions",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TeamId",
                table: "Transactions",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftOrders");

            migrationBuilder.DropTable(
                name: "DraftPicks");

            migrationBuilder.DropTable(
                name: "GameResults");

            migrationBuilder.DropTable(
                name: "LeagueMembers");

            migrationBuilder.DropTable(
                name: "MlbAllstars");

            migrationBuilder.DropTable(
                name: "MlbBattings");

            migrationBuilder.DropTable(
                name: "MlbFieldings");

            migrationBuilder.DropTable(
                name: "MlbHallOfFames");

            migrationBuilder.DropTable(
                name: "MlbPitchings");

            migrationBuilder.DropTable(
                name: "MlbTeams");

            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropTable(
                name: "RosterEntries");

            migrationBuilder.DropTable(
                name: "ScoutingReports");

            migrationBuilder.DropTable(
                name: "StandingsEntries");

            migrationBuilder.DropTable(
                name: "StratPlayers");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "MlbPeople");

            migrationBuilder.DropTable(
                name: "StratTeams");

            migrationBuilder.DropTable(
                name: "Drafts");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
