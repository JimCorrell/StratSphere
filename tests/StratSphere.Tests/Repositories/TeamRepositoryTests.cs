using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Data;
using StratSphere.Data.Repositories;

namespace StratSphere.Tests.Repositories;

public class TeamRepositoryTests : IDisposable
{
    private readonly StratSphereDbContext _db;
    private readonly TeamRepository _sut;

    public TeamRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StratSphereDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new StratSphereDbContext(options);
        _sut = new TeamRepository(_db);
    }

    public void Dispose() => _db.Dispose();

    // ── helpers ───────────────────────────────────────────────────────────────

    private static League MakeLeague() =>
        new() { Id = Guid.NewGuid(), Name = "Test League", Slug = "test", CommissionerId = Guid.NewGuid(), Status = LeagueStatus.Setup };

    private static Season MakeSeason(Guid leagueId, int year = 2025) =>
        new() { Id = Guid.NewGuid(), LeagueId = leagueId, Name = $"Season {year}", CardYear = year };

    private static Team MakeTeam(Guid leagueId, Guid? userId = null) =>
        new() { Id = Guid.NewGuid(), LeagueId = leagueId, UserId = userId, Name = "Test Team", City = "Springfield", Abbreviation = "TST" };

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsTeam()
    {
        var league = MakeLeague();
        var team = MakeTeam(league.Id);
        _db.Leagues.Add(league);
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(team.Id);

        Assert.NotNull(result);
        Assert.Equal(team.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotFound()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ── GetBySeasonIdAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetBySeasonIdAsync_ReturnsTeamsWithRosterSlotsInSeason()
    {
        var league = MakeLeague();
        var season = MakeSeason(league.Id);
        var team = MakeTeam(league.Id);
        var otherTeam = MakeTeam(league.Id); // no roster slot in this season

        _db.Leagues.Add(league);
        _db.Seasons.Add(season);
        _db.Teams.AddRange(team, otherTeam);
        await _db.SaveChangesAsync();

        // Give team a roster slot in the season
        var card = new PlayerCard { Id = Guid.NewGuid(), LahmanPlayerId = "ruthba01", CardYear = 2025, Position = "OF" };
        _db.PlayerCards.Add(card);
        _db.RosterSlots.Add(new RosterSlot { Id = Guid.NewGuid(), TeamId = team.Id, SeasonId = season.Id, CardId = card.Id, AcquiredAt = DateTimeOffset.UtcNow });
        await _db.SaveChangesAsync();

        var results = (await _sut.GetBySeasonIdAsync(season.Id)).ToList();

        Assert.Single(results);
        Assert.Equal(team.Id, results[0].Id);
    }

    [Fact]
    public async Task GetBySeasonIdAsync_ReturnsEmptyWhenNoRosterSlotsInSeason()
    {
        var league = MakeLeague();
        var season = MakeSeason(league.Id);
        _db.Leagues.Add(league);
        _db.Seasons.Add(season);
        await _db.SaveChangesAsync();

        var results = await _sut.GetBySeasonIdAsync(season.Id);

        Assert.Empty(results);
    }

    // ── GetByUserAndLeagueAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByUserAndLeagueAsync_ReturnsMatchingTeam()
    {
        var league = MakeLeague();
        var userId = Guid.NewGuid();
        var team = MakeTeam(league.Id, userId);
        _db.Leagues.Add(league);
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByUserAndLeagueAsync(userId, league.Id);

        Assert.NotNull(result);
        Assert.Equal(team.Id, result.Id);
    }

    [Fact]
    public async Task GetByUserAndLeagueAsync_ReturnsNullWhenUserHasNoTeamInLeague()
    {
        var league = MakeLeague();
        _db.Leagues.Add(league);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByUserAndLeagueAsync(Guid.NewGuid(), league.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserAndLeagueAsync_DoesNotReturnTeamInDifferentLeague()
    {
        var leagueA = MakeLeague();
        var leagueB = new League { Id = Guid.NewGuid(), Name = "Other", Slug = "other", CommissionerId = Guid.NewGuid(), Status = LeagueStatus.Setup };
        var userId = Guid.NewGuid();
        var team = MakeTeam(leagueA.Id, userId); // team is in leagueA

        _db.Leagues.AddRange(leagueA, leagueB);
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByUserAndLeagueAsync(userId, leagueB.Id); // query leagueB

        Assert.Null(result);
    }

    // ── ClaimAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task ClaimAsync_ReturnsTrueAndAssignsUser()
    {
        var league = MakeLeague();
        var team = MakeTeam(league.Id, userId: null); // unclaimed
        var userId = Guid.NewGuid();
        _db.Leagues.Add(league);
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        var result = await _sut.ClaimAsync(team.Id, userId, league.Id);

        Assert.True(result);
        var updated = await _db.Teams.FindAsync(team.Id);
        Assert.Equal(userId, updated!.UserId);
    }

    [Fact]
    public async Task ClaimAsync_ReturnsFalseWhenTeamAlreadyClaimed()
    {
        var league = MakeLeague();
        var existingOwner = Guid.NewGuid();
        var team = MakeTeam(league.Id, existingOwner); // already claimed
        _db.Leagues.Add(league);
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        var result = await _sut.ClaimAsync(team.Id, Guid.NewGuid(), league.Id);

        Assert.False(result);
        var unchanged = await _db.Teams.FindAsync(team.Id);
        Assert.Equal(existingOwner, unchanged!.UserId);
    }

    [Fact]
    public async Task ClaimAsync_ReturnsFalseForWrongLeague()
    {
        var leagueA = MakeLeague();
        var leagueB = new League { Id = Guid.NewGuid(), Name = "Other", Slug = "other", CommissionerId = Guid.NewGuid(), Status = LeagueStatus.Setup };
        var team = MakeTeam(leagueA.Id, userId: null);
        _db.Leagues.AddRange(leagueA, leagueB);
        _db.Teams.Add(team);
        await _db.SaveChangesAsync();

        // Pass leagueB.Id even though team belongs to leagueA
        var result = await _sut.ClaimAsync(team.Id, Guid.NewGuid(), leagueB.Id);

        Assert.False(result);
    }

    // ── AddAsync / SaveChangesAsync ───────────────────────────────────────────

    [Fact]
    public async Task AddAsync_PersistsTeam()
    {
        var league = MakeLeague();
        _db.Leagues.Add(league);
        await _db.SaveChangesAsync();

        var team = MakeTeam(league.Id);
        await _sut.AddAsync(team);
        await _sut.SaveChangesAsync();

        Assert.Equal(1, await _db.Teams.CountAsync());
    }
}
