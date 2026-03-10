using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;

namespace StratSphere.Tests.Services;

public class StandingsServiceTests
{
    private readonly Mock<IStandingsRepository> _standingsRepoMock;
    private readonly StandingsService _sut;

    public StandingsServiceTests()
    {
        _standingsRepoMock = new Mock<IStandingsRepository>();
        _sut = new StandingsService(_standingsRepoMock.Object);
    }

    private static Team MakeTeam() => new() { Id = Guid.NewGuid(), Name = "Test", City = "City", Abbreviation = "TST" };

    private static Game MakeGame(Guid homeId, Guid awayId, int homeScore, int awayScore,
        GameStatus status = GameStatus.Completed) => new()
    {
        Id = Guid.NewGuid(),
        HomeTeamId = homeId,
        AwayTeamId = awayId,
        HomeScore = homeScore,
        AwayScore = awayScore,
        Status = status,
        GameDate = DateOnly.FromDateTime(DateTime.Today)
    };

    [Fact]
    public async Task RecalculateAsync_CreditsWinToHomeTeamWhenHomeScoreHigher()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[] { MakeGame(home.Id, away.Id, 5, 3) };
        var teams = new[] { home, away };

        Standings? homeStandings = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => { if (s.TeamId == home.Id) homeStandings = s; })
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.NotNull(homeStandings);
        Assert.Equal(1, homeStandings!.Wins);
        Assert.Equal(0, homeStandings.Losses);
    }

    [Fact]
    public async Task RecalculateAsync_CreditsLossToAwayTeamWhenHomeScoreHigher()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[] { MakeGame(home.Id, away.Id, 5, 3) };
        var teams = new[] { home, away };

        Standings? awayStandings = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => { if (s.TeamId == away.Id) awayStandings = s; })
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.NotNull(awayStandings);
        Assert.Equal(0, awayStandings!.Wins);
        Assert.Equal(1, awayStandings.Losses);
    }

    [Fact]
    public async Task RecalculateAsync_CreditsWinToAwayTeamWhenAwayScoreHigher()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[] { MakeGame(home.Id, away.Id, 2, 7) };
        var teams = new[] { home, away };

        Standings? awayStandings = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => { if (s.TeamId == away.Id) awayStandings = s; })
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.Equal(1, awayStandings!.Wins);
        Assert.Equal(0, awayStandings.Losses);
    }

    [Fact]
    public async Task RecalculateAsync_CreditsTieToBothTeams()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[] { MakeGame(home.Id, away.Id, 4, 4) };
        var teams = new[] { home, away };

        var captured = new List<Standings>();
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => captured.Add(s))
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.All(captured, s => Assert.Equal(1, s.Ties));
        Assert.All(captured, s => Assert.Equal(0, s.Wins));
        Assert.All(captured, s => Assert.Equal(0, s.Losses));
    }

    [Fact]
    public async Task RecalculateAsync_AccumulatesRunsScoredAndAllowed()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[]
        {
            MakeGame(home.Id, away.Id, 5, 3),
            MakeGame(home.Id, away.Id, 2, 6)
        };
        var teams = new[] { home, away };

        Standings? homeStandings = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => { if (s.TeamId == home.Id) homeStandings = s; })
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.Equal(7, homeStandings!.RunsScored);   // 5 + 2
        Assert.Equal(9, homeStandings.RunsAllowed);   // 3 + 6
    }

    [Fact]
    public async Task RecalculateAsync_SkipsScheduledGames()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[]
        {
            MakeGame(home.Id, away.Id, 5, 3, GameStatus.Scheduled)
        };
        var teams = new[] { home, away };

        Standings? homeStandings = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => { if (s.TeamId == home.Id) homeStandings = s; })
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.Equal(0, homeStandings!.Wins);
        Assert.Equal(0, homeStandings.Losses);
        Assert.Equal(0, homeStandings.RunsScored);
    }

    [Fact]
    public async Task RecalculateAsync_SkipsPostponedGames()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var games = new[]
        {
            MakeGame(home.Id, away.Id, 0, 0, GameStatus.Postponed)
        };
        var teams = new[] { home, away };

        Standings? homeStandings = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => { if (s.TeamId == home.Id) homeStandings = s; })
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        Assert.Equal(0, homeStandings!.Wins);
        Assert.Equal(0, homeStandings.Losses);
    }

    [Fact]
    public async Task RecalculateAsync_IgnoresGamesWithTeamsNotInList()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var away = MakeTeam();
        var unknownTeam = MakeTeam();
        // Game references unknownTeam which is not in the teams list
        var games = new[] { MakeGame(home.Id, unknownTeam.Id, 5, 3) };
        var teams = new[] { home, away };

        var captured = new List<Standings>();
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => captured.Add(s))
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, games, teams);

        // No wins/losses should be recorded since away team not in map
        Assert.All(captured, s => Assert.Equal(0, s.Wins));
        Assert.All(captured, s => Assert.Equal(0, s.Losses));
    }

    [Fact]
    public async Task RecalculateAsync_UpsertsStandingsForAllTeams()
    {
        var seasonId = Guid.NewGuid();
        var teams = new[] { MakeTeam(), MakeTeam(), MakeTeam() };

        await _sut.RecalculateAsync(seasonId, [], teams);

        _standingsRepoMock.Verify(r => r.UpsertAsync(It.IsAny<Standings>()), Times.Exactly(3));
    }

    [Fact]
    public async Task RecalculateAsync_CallsSaveChangesOnce()
    {
        var seasonId = Guid.NewGuid();
        var teams = new[] { MakeTeam(), MakeTeam() };

        await _sut.RecalculateAsync(seasonId, [], teams);

        _standingsRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RecalculateAsync_SetsCorrectSeasonId()
    {
        var seasonId = Guid.NewGuid();
        var home = MakeTeam();
        var teams = new[] { home };

        Standings? captured = null;
        _standingsRepoMock
            .Setup(r => r.UpsertAsync(It.IsAny<Standings>()))
            .Callback<Standings>(s => captured = s)
            .Returns(Task.CompletedTask);

        await _sut.RecalculateAsync(seasonId, [], teams);

        Assert.Equal(seasonId, captured!.SeasonId);
    }
}
