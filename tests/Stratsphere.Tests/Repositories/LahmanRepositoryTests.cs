using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities.Lahman;
using StratSphere.Data;
using StratSphere.Data.Repositories;

namespace StratSphere.Tests.Repositories;

public class LahmanRepositoryTests : IDisposable
{
    private readonly StratSphereDbContext _db;
    private readonly LahmanRepository _sut;

    public LahmanRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<StratSphereDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new StratSphereDbContext(options);
        _sut = new LahmanRepository(_db);
    }

    public void Dispose() => _db.Dispose();

    // ── helpers ──────────────────────────────────────────────────────────────

    private static LahmanPerson Person(string id, string first, string last) =>
        new() { PlayerId = id, NameFirst = first, NameLast = last };

    private static LahmanBatting Batting(string playerId, int year, int ab, int stint = 1) =>
        new() { PlayerId = playerId, YearId = year, Stint = stint, AB = ab };

    private static LahmanPitching Pitching(string playerId, int year, int g, int gs = 0, int stint = 1) =>
        new() { PlayerId = playerId, YearId = year, Stint = stint, G = g, GS = gs };

    private static LahmanFielding Fielding(string playerId, int year, string pos, int games, int stint = 1) =>
        new() { PlayerId = playerId, YearId = year, Stint = stint, Pos = pos, G = games };

    // ── SearchCardsAsync — batter branch ─────────────────────────────────────

    [Fact]
    public async Task SearchCardsAsync_ReturnsBattersMatchingName()
    {
        _db.LahmanPeople.AddRange(
            Person("ruthba01", "Babe", "Ruth"),
            Person("gehrig01", "Lou", "Gehrig"));
        _db.LahmanBatting.AddRange(
            Batting("ruthba01", 1927, 540),
            Batting("gehrig01", 1927, 584));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("ruth", null)).ToList();

        Assert.Single(results);
        Assert.Equal("ruthba01", results[0].Person.PlayerId);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_IsCaseInsensitive()
    {
        _db.LahmanPeople.Add(Person("ruthba01", "Babe", "Ruth"));
        _db.LahmanBatting.Add(Batting("ruthba01", 1927, 540));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("RUTH", null)).ToList();

        Assert.Single(results);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_FiltersByCardYear()
    {
        _db.LahmanPeople.AddRange(
            Person("ruthba01", "Babe", "Ruth"),
            Person("mayswi01", "Willie", "Mays"));
        _db.LahmanBatting.AddRange(
            Batting("ruthba01", 1927, 540),
            Batting("mayswi01", 1954, 543));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("a", 1927)).ToList();

        Assert.Single(results);
        Assert.Equal("ruthba01", results[0].Person.PlayerId);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_WithNullCardYear_ReturnsAcrossAllYears()
    {
        _db.LahmanPeople.AddRange(
            Person("ruthba01", "Babe", "Ruth"),
            Person("mayswi01", "Willie", "Mays"));
        _db.LahmanBatting.AddRange(
            Batting("ruthba01", 1927, 540),
            Batting("mayswi01", 1954, 543));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("a", null)).ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_ExcludesPlayersBelow51AB()
    {
        _db.LahmanPeople.AddRange(
            Person("ruthba01", "Babe", "Ruth"),
            Person("bench01", "Johnny", "Bench"));
        _db.LahmanBatting.AddRange(
            Batting("ruthba01", 1927, 540),
            Batting("bench01", 1927, 30));    // below threshold
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("a", null)).ToList();

        Assert.Single(results);
        Assert.Equal("ruthba01", results[0].Person.PlayerId);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_UsesFieldingPositionWithMostGames()
    {
        _db.LahmanPeople.Add(Person("ruthba01", "Babe", "Ruth"));
        _db.LahmanBatting.Add(Batting("ruthba01", 1927, 540));
        _db.LahmanFielding.AddRange(
            Fielding("ruthba01", 1927, "OF", 100),
            Fielding("ruthba01", 1927, "1B", 40));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("ruth", null)).ToList();

        Assert.Single(results);
        Assert.Equal("OF", results[0].PrimaryPosition);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_FallsBackToOFWhenNoFieldingData()
    {
        _db.LahmanPeople.Add(Person("ruthba01", "Babe", "Ruth"));
        _db.LahmanBatting.Add(Batting("ruthba01", 1927, 540));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("ruth", null)).ToList();

        Assert.Single(results);
        Assert.Equal("OF", results[0].PrimaryPosition);
    }

    [Fact]
    public async Task SearchCardsAsync_BatterSearch_RespectsLimit()
    {
        for (var i = 0; i < 10; i++)
        {
            var id = $"player{i:D2}";
            _db.LahmanPeople.Add(Person(id, "Test", $"Player{i:D2}"));
            _db.LahmanBatting.Add(Batting(id, 1990, 200));
        }
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("player", null, pitchersOnly: false, limit: 3)).ToList();

        Assert.Equal(3, results.Count);
    }

    // ── SearchCardsAsync — pitcher branch ────────────────────────────────────

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_ReturnsPitchersMatchingName()
    {
        _db.LahmanPeople.AddRange(
            Person("koufasa01", "Sandy", "Koufax"),
            Person("ruthba01", "Babe", "Ruth"));
        _db.LahmanPitching.AddRange(
            Pitching("koufasa01", 1965, g: 43, gs: 41),
            Pitching("ruthba01", 1927, g: 0, gs: 0));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("koufax", null, pitchersOnly: true)).ToList();

        Assert.Single(results);
        Assert.Equal("koufasa01", results[0].Person.PlayerId);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_AssignsSPPosition()
    {
        _db.LahmanPeople.Add(Person("koufasa01", "Sandy", "Koufax"));
        _db.LahmanPitching.Add(Pitching("koufasa01", 1965, g: 43, gs: 41));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("koufax", null, pitchersOnly: true)).ToList();

        Assert.Equal("SP", results[0].PrimaryPosition);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_FiltersByCardYear()
    {
        _db.LahmanPeople.AddRange(
            Person("koufasa01", "Sandy", "Koufax"),
            Person("gibsobo01", "Bob", "Gibson"));
        _db.LahmanPitching.AddRange(
            Pitching("koufasa01", 1965, g: 43, gs: 41),
            Pitching("gibsobo01", 1968, g: 34, gs: 34));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("a", 1965, pitchersOnly: true)).ToList();

        Assert.Single(results);
        Assert.Equal("koufasa01", results[0].Person.PlayerId);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_WithNullCardYear_ReturnsAcrossAllYears()
    {
        _db.LahmanPeople.AddRange(
            Person("koufasa01", "Sandy", "Koufax"),
            Person("gibsobo01", "Bob", "Gibson"));
        _db.LahmanPitching.AddRange(
            Pitching("koufasa01", 1965, g: 43, gs: 41),
            Pitching("gibsobo01", 1968, g: 34, gs: 34));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("a", null, pitchersOnly: true)).ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_ExcludesInsufficientStats()
    {
        // GS == 0 && G <= 3 should be excluded
        _db.LahmanPeople.AddRange(
            Person("koufasa01", "Sandy", "Koufax"),
            Person("mopup01", "Mop", "Up"));
        _db.LahmanPitching.AddRange(
            Pitching("koufasa01", 1965, g: 43, gs: 41),
            Pitching("mopup01", 1965, g: 2, gs: 0));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("a", null, pitchersOnly: true)).ToList();

        Assert.Single(results);
        Assert.Equal("koufasa01", results[0].Person.PlayerId);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_IncludesRelieversWithMoreThan3Games()
    {
        // G > 3 with GS == 0 should be included (reliever)
        _db.LahmanPeople.Add(Person("eckersde01", "Dennis", "Eckersley"));
        _db.LahmanPitching.Add(Pitching("eckersde01", 1990, g: 63, gs: 0));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("eckersley", null, pitchersOnly: true)).ToList();

        Assert.Single(results);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_IsCaseInsensitive()
    {
        _db.LahmanPeople.Add(Person("koufasa01", "Sandy", "Koufax"));
        _db.LahmanPitching.Add(Pitching("koufasa01", 1965, g: 43, gs: 41));
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("KOUFAX", null, pitchersOnly: true)).ToList();

        Assert.Single(results);
    }

    [Fact]
    public async Task SearchCardsAsync_WithPitchersOnly_RespectsLimit()
    {
        for (var i = 0; i < 10; i++)
        {
            var id = $"ptchr{i:D3}";
            _db.LahmanPeople.Add(Person(id, "Test", $"Pitcher{i:D2}"));
            _db.LahmanPitching.Add(Pitching(id, 1990, g: 30, gs: 15));
        }
        await _db.SaveChangesAsync();

        var results = (await _sut.SearchCardsAsync("pitcher", null, pitchersOnly: true, limit: 4)).ToList();

        Assert.Equal(4, results.Count);
    }
}
