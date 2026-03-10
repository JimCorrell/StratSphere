using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;

namespace StratSphere.Tests.Services;

public class RosterServiceTests
{
    private readonly Mock<IRosterRepository> _rosterRepoMock;
    private readonly Mock<IPlayerCardRepository> _cardRepoMock;
    private readonly RosterService _sut;

    public RosterServiceTests()
    {
        _rosterRepoMock = new Mock<IRosterRepository>();
        _cardRepoMock = new Mock<IPlayerCardRepository>();
        _sut = new RosterService(_rosterRepoMock.Object, _cardRepoMock.Object);
    }

    [Fact]
    public async Task AddCardToRosterAsync_ReturnsSlotWithCorrectTeamAndSeason()
    {
        var teamId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var card = new PlayerCard { Id = Guid.NewGuid(), LahmanPlayerId = "ruthba01", CardYear = 1927, Position = "1B" };

        _cardRepoMock.Setup(r => r.GetOrCreateAsync("ruthba01", 1927, "1B")).ReturnsAsync(card);
        _rosterRepoMock.Setup(r => r.CardIsRosteredInSeasonAsync(card.Id, seasonId)).ReturnsAsync(false);

        var result = await _sut.AddCardToRosterAsync(teamId, seasonId, "ruthba01", 1927, "1B");

        Assert.Equal(teamId, result.TeamId);
        Assert.Equal(seasonId, result.SeasonId);
        Assert.Equal(card.Id, result.CardId);
    }

    [Fact]
    public async Task AddCardToRosterAsync_SetsSlotTypeToActive()
    {
        var card = new PlayerCard { Id = Guid.NewGuid(), LahmanPlayerId = "ruthba01", CardYear = 1927, Position = "1B" };
        _cardRepoMock.Setup(r => r.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(card);
        _rosterRepoMock.Setup(r => r.CardIsRosteredInSeasonAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _sut.AddCardToRosterAsync(Guid.NewGuid(), Guid.NewGuid(), "ruthba01", 1927, "1B");

        Assert.Equal("active", result.SlotType);
    }

    [Fact]
    public async Task AddCardToRosterAsync_ThrowsWhenCardAlreadyRosteredInSeason()
    {
        var card = new PlayerCard { Id = Guid.NewGuid(), LahmanPlayerId = "ruthba01", CardYear = 1927, Position = "1B" };
        _cardRepoMock.Setup(r => r.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(card);
        _rosterRepoMock.Setup(r => r.CardIsRosteredInSeasonAsync(card.Id, It.IsAny<Guid>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.AddCardToRosterAsync(Guid.NewGuid(), Guid.NewGuid(), "ruthba01", 1927, "1B"));
    }

    [Fact]
    public async Task AddCardToRosterAsync_CallsAddAndSaveChanges()
    {
        var card = new PlayerCard { Id = Guid.NewGuid(), LahmanPlayerId = "ruthba01", CardYear = 1927, Position = "1B" };
        _cardRepoMock.Setup(r => r.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(card);
        _rosterRepoMock.Setup(r => r.CardIsRosteredInSeasonAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(false);

        await _sut.AddCardToRosterAsync(Guid.NewGuid(), Guid.NewGuid(), "ruthba01", 1927, "1B");

        _rosterRepoMock.Verify(r => r.AddAsync(It.IsAny<RosterSlot>()), Times.Once);
        _rosterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DropCardFromRosterAsync_CallsDropWithCorrectArguments()
    {
        var slotId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        await _sut.DropCardFromRosterAsync(slotId, teamId);

        _rosterRepoMock.Verify(r => r.DropAsync(slotId, teamId), Times.Once);
    }

    [Fact]
    public async Task DropCardFromRosterAsync_CallsSaveChanges()
    {
        await _sut.DropCardFromRosterAsync(Guid.NewGuid(), Guid.NewGuid());

        _rosterRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
