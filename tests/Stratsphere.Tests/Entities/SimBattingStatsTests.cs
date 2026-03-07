using StratSphere.Core.Entities;

namespace StratSphere.Tests.Entities;

public class SimBattingStatsTests
{
    [Fact]
    public void BA_IsNullWhenNoAtBats()
    {
        var stats = new SimBattingStats { AB = 0, H = 0 };
        Assert.Null(stats.BA);
    }

    [Fact]
    public void BA_CalculatesCorrectly()
    {
        var stats = new SimBattingStats { AB = 100, H = 30 };
        Assert.Equal(0.300m, stats.BA);
    }

    [Fact]
    public void OBP_IsNullWhenDenominatorIsZero()
    {
        var stats = new SimBattingStats { AB = 0, BB = 0, HBP = 0, SF = 0 };
        Assert.Null(stats.OBP);
    }

    [Fact]
    public void OBP_CalculatesCorrectly()
    {
        // (H + BB + HBP) / (AB + BB + HBP + SF)
        // (30 + 10 + 2) / (100 + 10 + 2 + 1) = 42/113 = .372
        var stats = new SimBattingStats { AB = 100, H = 30, BB = 10, HBP = 2, SF = 1 };
        Assert.Equal(0.372m, stats.OBP);
    }

    [Fact]
    public void SLG_IsNullWhenNoAtBats()
    {
        var stats = new SimBattingStats { AB = 0 };
        Assert.Null(stats.SLG);
    }

    [Fact]
    public void SLG_CalculatesCorrectly()
    {
        // TB = singles + 2*2B + 3*3B + 4*HR
        // With H=10, 2B=2, 3B=1, HR=3, AB=20
        // singles = 10-2-1-3 = 4
        // TB = 4*1 + 2*2 + 1*3 + 3*4 = 4+4+3+12 = 23
        // SLG = 23/20 = 1.150
        var stats = new SimBattingStats { AB = 20, H = 10, Doubles = 2, Triples = 1, HR = 3 };
        Assert.Equal(1.150m, stats.SLG);
    }

    [Fact]
    public void OPS_IsOBPPlusSLG()
    {
        var stats = new SimBattingStats { AB = 100, H = 30, BB = 10, Doubles = 5, Triples = 1, HR = 4 };
        Assert.Equal(stats.OBP + stats.SLG, stats.OPS);
    }

    [Fact]
    public void OPS_IsNullWhenNoAtBats()
    {
        var stats = new SimBattingStats { AB = 0 };
        Assert.Null(stats.OPS);
    }
}
