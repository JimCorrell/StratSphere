using StratSphere.Core.Entities;

namespace StratSphere.Tests.Entities;

public class StandingsTests
{
    [Fact]
    public void GamesPlayed_IsSumOfWinsLossesTies()
    {
        var standings = new Standings { Wins = 10, Losses = 8, Ties = 2 };
        Assert.Equal(20, standings.GamesPlayed);
    }

    [Fact]
    public void GamesPlayed_IsZeroWhenNoGames()
    {
        var standings = new Standings();
        Assert.Equal(0, standings.GamesPlayed);
    }

    [Fact]
    public void WinPct_IsZeroWhenNoGamesPlayed()
    {
        var standings = new Standings { Wins = 0, Losses = 0, Ties = 0 };
        Assert.Equal(0m, standings.WinPct);
    }

    [Fact]
    public void WinPct_IsOneForPerfectRecord()
    {
        var standings = new Standings { Wins = 10, Losses = 0, Ties = 0 };
        Assert.Equal(1.000m, standings.WinPct);
    }

    [Fact]
    public void WinPct_IsHalfForFiftyFiftyRecord()
    {
        var standings = new Standings { Wins = 5, Losses = 5, Ties = 0 };
        Assert.Equal(0.500m, standings.WinPct);
    }

    [Fact]
    public void WinPct_CountsTiesAsHalfWin()
    {
        // 0W 0L 10T => WinPct = (0 + 5) / 10 = 0.500
        var standings = new Standings { Wins = 0, Losses = 0, Ties = 10 };
        Assert.Equal(0.500m, standings.WinPct);
    }

    [Fact]
    public void WinPct_IsRoundedToThreeDecimalPlaces()
    {
        // 1W 2L 0T => 1/3 = 0.333
        var standings = new Standings { Wins = 1, Losses = 2, Ties = 0 };
        Assert.Equal(0.333m, standings.WinPct);
    }

    [Fact]
    public void RunDiff_IsPositiveWhenScoredMoreThanAllowed()
    {
        var standings = new Standings { RunsScored = 100, RunsAllowed = 80 };
        Assert.Equal(20, standings.RunDiff);
    }

    [Fact]
    public void RunDiff_IsNegativeWhenAllowedMoreThanScored()
    {
        var standings = new Standings { RunsScored = 60, RunsAllowed = 90 };
        Assert.Equal(-30, standings.RunDiff);
    }

    [Fact]
    public void RunDiff_IsZeroWhenEqual()
    {
        var standings = new Standings { RunsScored = 75, RunsAllowed = 75 };
        Assert.Equal(0, standings.RunDiff);
    }
}
