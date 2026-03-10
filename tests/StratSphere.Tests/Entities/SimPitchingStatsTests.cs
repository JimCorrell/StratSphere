using StratSphere.Core.Entities;

namespace StratSphere.Tests.Entities;

public class SimPitchingStatsTests
{
    [Fact]
    public void IP_IsNullWhenIPOutsIsZero()
    {
        var stats = new SimPitchingStats { IPOuts = 0 };
        Assert.Null(stats.IP);
    }

    [Fact]
    public void IP_CalculatesCorrectly()
    {
        var stats = new SimPitchingStats { IPOuts = 90 };
        Assert.Equal(30.0m, stats.IP);
    }

    [Fact]
    public void ERA_IsNullWhenIPOutsIsZero()
    {
        var stats = new SimPitchingStats { IPOuts = 0, ER = 10 };
        Assert.Null(stats.ERA);
    }

    [Fact]
    public void ERA_CalculatesCorrectly()
    {
        // ERA = ER / IPOuts * 27
        // 30 ER in 90 outs (30 IP) => 30/90*27 = 9.00
        var stats = new SimPitchingStats { IPOuts = 90, ER = 30 };
        Assert.Equal(9.00m, stats.ERA);
    }

    [Fact]
    public void WHIP_IsNullWhenIPOutsIsZero()
    {
        var stats = new SimPitchingStats { IPOuts = 0 };
        Assert.Null(stats.WHIP);
    }

    [Fact]
    public void WHIP_CalculatesCorrectly()
    {
        // WHIP = (BB + H) / IPOuts * 3
        // (10 + 20) / 90 * 3 = 30/90*3 = 1.000
        var stats = new SimPitchingStats { IPOuts = 90, BB = 10, H = 20 };
        Assert.Equal(1.000m, stats.WHIP);
    }

    [Fact]
    public void K9_IsNullWhenIPOutsIsZero()
    {
        var stats = new SimPitchingStats { IPOuts = 0 };
        Assert.Null(stats.K9);
    }

    [Fact]
    public void K9_CalculatesCorrectly()
    {
        // K/9 = SO / IPOuts * 27
        // 90 SO in 90 outs (30 IP) => 90/90*27 = 27.00
        var stats = new SimPitchingStats { IPOuts = 90, SO = 90 };
        Assert.Equal(27.00m, stats.K9);
    }
}
