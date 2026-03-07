using StratSphere.Core.Entities.Lahman;

namespace StratSphere.Tests.Entities;

public class LahmanBattingTests
{
    [Fact]
    public void BA_IsNullWhenNoAtBats()
    {
        var batting = new LahmanBatting { AB = 0, H = 0 };
        Assert.Null(batting.BA);
    }

    [Fact]
    public void BA_IsNullWhenABIsNull()
    {
        var batting = new LahmanBatting { AB = null, H = 100 };
        Assert.Null(batting.BA);
    }

    [Fact]
    public void BA_CalculatesCorrectly()
    {
        // 192 H / 540 AB = .356
        var batting = new LahmanBatting { AB = 540, H = 192 };
        Assert.Equal(0.356m, batting.BA);
    }

    [Fact]
    public void OBP_IsNullWhenDenominatorIsZero()
    {
        var batting = new LahmanBatting { AB = 0, BB = 0, HBP = 0, SF = 0 };
        Assert.Null(batting.OBP);
    }

    [Fact]
    public void OBP_CalculatesCorrectly()
    {
        // (H + BB + HBP) / (AB + BB + HBP + SF)
        // (192 + 130 + 0) / (540 + 130 + 0 + 0) = 322/670 = .481
        var batting = new LahmanBatting { AB = 540, H = 192, BB = 130, HBP = 0, SF = 0 };
        Assert.Equal(0.481m, batting.OBP);
    }

    [Fact]
    public void SLG_IsNullWhenNoAtBats()
    {
        var batting = new LahmanBatting { AB = 0 };
        Assert.Null(batting.SLG);
    }

    [Fact]
    public void SLG_CalculatesCorrectly()
    {
        // TB = H + 2B + 2*3B + 3*HR
        // H=10, 2B=2, 3B=1, HR=3, AB=20
        // TB = 10 + 2 + 2 + 9 = 23
        // SLG = 23/20 = 1.150
        var batting = new LahmanBatting { AB = 20, H = 10, Doubles = 2, Triples = 1, HR = 3 };
        Assert.Equal(1.150m, batting.SLG);
    }

    [Fact]
    public void OPS_IsNullWhenOBPIsNull()
    {
        var batting = new LahmanBatting { AB = 0 };
        Assert.Null(batting.OPS);
    }

    [Fact]
    public void OPS_IsNullWhenSLGIsNull()
    {
        var batting = new LahmanBatting { AB = null, BB = 10 };
        Assert.Null(batting.OPS);
    }

    [Fact]
    public void OPS_IsOBPPlusSLG()
    {
        var batting = new LahmanBatting { AB = 100, H = 30, BB = 10, Doubles = 5, Triples = 1, HR = 4, HBP = 0, SF = 0 };
        Assert.Equal(batting.OBP + batting.SLG, batting.OPS);
    }
}
