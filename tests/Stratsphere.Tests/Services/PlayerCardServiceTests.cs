using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Entities.Lahman;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;

namespace StratSphere.Tests.Services;

public class PlayerCardServiceTests
{
    private readonly Mock<ILahmanRepository> _lahmanRepoMock;
    private readonly PlayerCardService _sut;

    public PlayerCardServiceTests()
    {
        _lahmanRepoMock = new Mock<ILahmanRepository>();
        _sut = new PlayerCardService(_lahmanRepoMock.Object);
    }

    private static LahmanPerson MakePerson(string id = "ruthba01") => new()
    {
        PlayerId = id,
        NameFirst = "Babe",
        NameLast = "Ruth"
    };

    private static PlayerCard MakeCard(string position, int year = 1927) => new()
    {
        Id = Guid.NewGuid(),
        LahmanPlayerId = "ruthba01",
        CardYear = year,
        Position = position
    };

    [Fact]
    public async Task GetCardStatsAsync_ReturnsNullWhenPersonNotFound()
    {
        _lahmanRepoMock.Setup(r => r.GetPersonAsync(It.IsAny<string>())).ReturnsAsync((LahmanPerson?)null);

        var result = await _sut.GetCardStatsAsync(MakeCard("1B"), null, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCardStatsAsync_ReturnsCardInResult()
    {
        var card = MakeCard("1B");
        var person = MakePerson();
        _lahmanRepoMock.Setup(r => r.GetPersonAsync("ruthba01")).ReturnsAsync(person);
        _lahmanRepoMock.Setup(r => r.GetBattingSeasonAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new LahmanBatting { PlayerId = "ruthba01", YearId = 1927 });

        var result = await _sut.GetCardStatsAsync(card, null, null);

        Assert.Equal(card, result!.Card);
        Assert.Equal(person, result.Person);
    }

    [Fact]
    public async Task GetCardStatsAsync_LoadsBattingStatsForNonPitcher()
    {
        var card = MakeCard("1B", 1927);
        var batting = new LahmanBatting { PlayerId = "ruthba01", YearId = 1927, H = 192, AB = 540 };
        _lahmanRepoMock.Setup(r => r.GetPersonAsync("ruthba01")).ReturnsAsync(MakePerson());
        _lahmanRepoMock.Setup(r => r.GetBattingSeasonAsync("ruthba01", 1927)).ReturnsAsync(batting);

        var result = await _sut.GetCardStatsAsync(card, null, null);

        Assert.Equal(batting, result!.HistoricalBatting);
        Assert.Null(result.HistoricalPitching);
    }

    [Fact]
    public async Task GetCardStatsAsync_DoesNotLoadBattingForSP()
    {
        var card = MakeCard("SP", 1965);
        var pitching = new LahmanPitching { PlayerId = "ruthba01", YearId = 1965 };
        _lahmanRepoMock.Setup(r => r.GetPersonAsync("ruthba01")).ReturnsAsync(MakePerson());
        _lahmanRepoMock.Setup(r => r.GetPitchingSeasonAsync("ruthba01", 1965)).ReturnsAsync(pitching);

        var result = await _sut.GetCardStatsAsync(card, null, null);

        Assert.Null(result!.HistoricalBatting);
        _lahmanRepoMock.Verify(r => r.GetBattingSeasonAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetCardStatsAsync_LoadsPitchingStatsForSP()
    {
        var card = MakeCard("SP", 1965);
        var pitching = new LahmanPitching { PlayerId = "ruthba01", YearId = 1965, W = 20, L = 10 };
        _lahmanRepoMock.Setup(r => r.GetPersonAsync("ruthba01")).ReturnsAsync(MakePerson());
        _lahmanRepoMock.Setup(r => r.GetPitchingSeasonAsync("ruthba01", 1965)).ReturnsAsync(pitching);

        var result = await _sut.GetCardStatsAsync(card, null, null);

        Assert.Equal(pitching, result!.HistoricalPitching);
    }

    [Fact]
    public async Task GetCardStatsAsync_LoadsPitchingStatsForRP()
    {
        var card = MakeCard("RP", 1980);
        var pitching = new LahmanPitching { PlayerId = "ruthba01", YearId = 1980, SV = 33 };
        _lahmanRepoMock.Setup(r => r.GetPersonAsync("ruthba01")).ReturnsAsync(MakePerson());
        _lahmanRepoMock.Setup(r => r.GetPitchingSeasonAsync("ruthba01", 1980)).ReturnsAsync(pitching);

        var result = await _sut.GetCardStatsAsync(card, null, null);

        Assert.Equal(pitching, result!.HistoricalPitching);
    }

    [Fact]
    public async Task GetCardStatsAsync_PassesThroughSimBattingStats()
    {
        var card = MakeCard("1B");
        var simBatting = new SimBattingStats { Id = Guid.NewGuid() };
        _lahmanRepoMock.Setup(r => r.GetPersonAsync(It.IsAny<string>())).ReturnsAsync(MakePerson());
        _lahmanRepoMock.Setup(r => r.GetBattingSeasonAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new LahmanBatting());

        var result = await _sut.GetCardStatsAsync(card, simBatting, null);

        Assert.Equal(simBatting, result!.SimBatting);
        Assert.Null(result.SimPitching);
    }

    [Fact]
    public async Task GetCardStatsAsync_PassesThroughSimPitchingStats()
    {
        var card = MakeCard("SP");
        var simPitching = new SimPitchingStats { Id = Guid.NewGuid() };
        _lahmanRepoMock.Setup(r => r.GetPersonAsync(It.IsAny<string>())).ReturnsAsync(MakePerson());
        _lahmanRepoMock.Setup(r => r.GetPitchingSeasonAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new LahmanPitching());

        var result = await _sut.GetCardStatsAsync(card, null, simPitching);

        Assert.Null(result!.SimBatting);
        Assert.Equal(simPitching, result.SimPitching);
    }

    [Theory]
    [InlineData("SP", true)]
    [InlineData("RP", true)]
    [InlineData("1B", false)]
    [InlineData("CF", false)]
    [InlineData("C", false)]
    public async Task GetCardStatsAsync_IsPitcherCorrect(string position, bool expectedIsPitcher)
    {
        var card = MakeCard(position);
        _lahmanRepoMock.Setup(r => r.GetPersonAsync(It.IsAny<string>())).ReturnsAsync(MakePerson());
        if (expectedIsPitcher)
            _lahmanRepoMock.Setup(r => r.GetPitchingSeasonAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new LahmanPitching());
        else
            _lahmanRepoMock.Setup(r => r.GetBattingSeasonAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new LahmanBatting());

        var result = await _sut.GetCardStatsAsync(card, null, null);

        Assert.Equal(expectedIsPitcher, result!.IsPitcher);
    }
}
