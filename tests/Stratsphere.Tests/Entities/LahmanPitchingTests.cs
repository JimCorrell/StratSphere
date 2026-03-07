using StratSphere.Core.Entities.Lahman;

namespace StratSphere.Tests.Entities;

public class LahmanPitchingTests
{
    [Fact]
    public void IP_IsNullWhenIPOutsIsNull()
    {
        var pitching = new LahmanPitching { IPOuts = null };
        Assert.Null(pitching.IP);
    }

    [Fact]
    public void IP_CalculatesCorrectly()
    {
        // 300 outs / 3 = 100.0 IP
        var pitching = new LahmanPitching { IPOuts = 300 };
        Assert.Equal(100.0m, pitching.IP);
    }

    [Fact]
    public void IP_RoundsToOneDecimalPlace()
    {
        // 10 outs = 3.1 IP (3 full innings + 1 out)
        var pitching = new LahmanPitching { IPOuts = 10 };
        Assert.Equal(3.3m, pitching.IP);
    }

    [Fact]
    public void ERA_IsNullWhenIPOutsIsNull()
    {
        var pitching = new LahmanPitching { IPOuts = null };
        Assert.Null(pitching.ERA);
    }

    [Fact]
    public void ERA_IsNullWhenIPOutsIsZero()
    {
        var pitching = new LahmanPitching { IPOuts = 0, ER = 5 };
        Assert.Null(pitching.ERA);
    }

    [Fact]
    public void ERA_CalculatesCorrectly()
    {
        // ERA = ER / IPOuts * 27
        // 100 ER in 300 outs (100 IP) => 100/300*27 = 9.00
        var pitching = new LahmanPitching { IPOuts = 300, ER = 100 };
        Assert.Equal(9.00m, pitching.ERA);
    }

    [Fact]
    public void WHIP_IsNullWhenIPOutsIsNotPositive()
    {
        var pitching = new LahmanPitching { IPOuts = 0 };
        Assert.Null(pitching.WHIP);
    }

    [Fact]
    public void WHIP_CalculatesCorrectly()
    {
        // WHIP = (BB + H) / IPOuts * 3
        // (30 + 90) / 300 * 3 = 120/300*3 = 1.200
        var pitching = new LahmanPitching { IPOuts = 300, BB = 30, H = 90 };
        Assert.Equal(1.200m, pitching.WHIP);
    }

    [Fact]
    public void K9_IsNullWhenIPOutsIsNotPositive()
    {
        var pitching = new LahmanPitching { IPOuts = null };
        Assert.Null(pitching.K9);
    }

    [Fact]
    public void K9_CalculatesCorrectly()
    {
        // K/9 = SO / IPOuts * 27
        // 200 SO in 300 outs (100 IP) => 200/300*27 = 18.00
        var pitching = new LahmanPitching { IPOuts = 300, SO = 200 };
        Assert.Equal(18.00m, pitching.K9);
    }
}
