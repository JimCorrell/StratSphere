using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Data;
using StratSphere.Data.Repositories;

namespace StratSphere.Tests.Repositories;

public class LeagueRepositoryTests : IDisposable
{
    private readonly StratSphereDbContext _db;
    private readonly LeagueRepository _sut;

    public LeagueRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StratSphereDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new StratSphereDbContext(options);
        _sut = new LeagueRepository(_db);
    }

    public void Dispose() => _db.Dispose();

    // ── helpers ───────────────────────────────────────────────────────────────

    private static League MakeLeague(string name, string slug, Guid? commissionerId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            CommissionerId = commissionerId ?? Guid.NewGuid(),
            Status = LeagueStatus.Setup
        };

    private static LeagueMember MakeMember(Guid leagueId, Guid userId, LeagueRole role = LeagueRole.Manager) =>
        new() { LeagueId = leagueId, UserId = userId, Role = role };

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsLeagueWithMembers()
    {
        var league = MakeLeague("Test League", "test-league");
        league.Members.Add(MakeMember(league.Id, Guid.NewGuid()));
        _db.Leagues.Add(league);
        await _db.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(league.Id);

        Assert.NotNull(result);
        Assert.Equal(league.Id, result.Id);
        Assert.Single(result.Members);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotFound()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ── GetBySlugAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetBySlugAsync_ReturnsLeagueMatchingSlug()
    {
        var league = MakeLeague("Alpha League", "alpha-league");
        _db.Leagues.Add(league);
        await _db.SaveChangesAsync();

        var result = await _sut.GetBySlugAsync("alpha-league");

        Assert.NotNull(result);
        Assert.Equal("alpha-league", result.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_ReturnsNullForUnknownSlug()
    {
        var result = await _sut.GetBySlugAsync("does-not-exist");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_DoesNotReturnWrongLeague()
    {
        _db.Leagues.Add(MakeLeague("Alpha", "alpha"));
        _db.Leagues.Add(MakeLeague("Beta", "beta"));
        await _db.SaveChangesAsync();

        var result = await _sut.GetBySlugAsync("beta");

        Assert.NotNull(result);
        Assert.Equal("beta", result.Slug);
    }

    // ── GetByUserIdAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByUserIdAsync_ReturnsLeaguesWhereUserIsMember()
    {
        var userId = Guid.NewGuid();
        var leagueA = MakeLeague("Zebra", "zebra");
        var leagueB = MakeLeague("Apple", "apple");
        var leagueC = MakeLeague("Other", "other"); // user NOT a member

        leagueA.Members.Add(MakeMember(leagueA.Id, userId));
        leagueB.Members.Add(MakeMember(leagueB.Id, userId));
        leagueC.Members.Add(MakeMember(leagueC.Id, Guid.NewGuid()));

        _db.Leagues.AddRange(leagueA, leagueB, leagueC);
        await _db.SaveChangesAsync();

        var results = (await _sut.GetByUserIdAsync(userId)).ToList();

        Assert.Equal(2, results.Count);
        Assert.DoesNotContain(results, l => l.Id == leagueC.Id);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsLeaguesOrderedByName()
    {
        var userId = Guid.NewGuid();
        var leagueZ = MakeLeague("Zebra League", "zebra");
        var leagueA = MakeLeague("Apple League", "apple");

        leagueZ.Members.Add(MakeMember(leagueZ.Id, userId));
        leagueA.Members.Add(MakeMember(leagueA.Id, userId));

        _db.Leagues.AddRange(leagueZ, leagueA);
        await _db.SaveChangesAsync();

        var results = (await _sut.GetByUserIdAsync(userId)).ToList();

        Assert.Equal("Apple League", results[0].Name);
        Assert.Equal("Zebra League", results[1].Name);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsEmptyWhenUserHasNoLeagues()
    {
        var results = await _sut.GetByUserIdAsync(Guid.NewGuid());

        Assert.Empty(results);
    }

    // ── SlugExistsAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task SlugExistsAsync_ReturnsTrueForExistingSlug()
    {
        _db.Leagues.Add(MakeLeague("Test", "test-slug"));
        await _db.SaveChangesAsync();

        var result = await _sut.SlugExistsAsync("test-slug");

        Assert.True(result);
    }

    [Fact]
    public async Task SlugExistsAsync_ReturnsFalseForUnknownSlug()
    {
        var result = await _sut.SlugExistsAsync("no-such-slug");

        Assert.False(result);
    }

    // ── AddAsync / SaveChangesAsync ───────────────────────────────────────────

    [Fact]
    public async Task AddAsync_PersistsLeague()
    {
        var league = MakeLeague("New League", "new-league");

        await _sut.AddAsync(league);
        await _sut.SaveChangesAsync();

        Assert.Equal(1, await _db.Leagues.CountAsync());
        Assert.Equal("new-league", (await _db.Leagues.FirstAsync()).Slug);
    }
}
