using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;

namespace StratSphere.Tests.Services;

public class LeagueServiceTests
{
    private readonly Mock<ILeagueRepository> _leagueRepoMock;
    private readonly LeagueService _sut;

    public LeagueServiceTests()
    {
        _leagueRepoMock = new Mock<ILeagueRepository>();
        _sut = new LeagueService(_leagueRepoMock.Object);
    }

    [Fact]
    public async Task CreateLeagueAsync_ReturnsLeagueWithCorrectName()
    {
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.CreateLeagueAsync("My League", Guid.NewGuid());

        Assert.Equal("My League", result.Name);
    }

    [Fact]
    public async Task CreateLeagueAsync_SetsCommissionerId()
    {
        var commissionerId = Guid.NewGuid();
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.CreateLeagueAsync("Test League", commissionerId);

        Assert.Equal(commissionerId, result.CommissionerId);
    }

    [Fact]
    public async Task CreateLeagueAsync_SetsStatusToSetup()
    {
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.CreateLeagueAsync("Test League", Guid.NewGuid());

        Assert.Equal("setup", result.Status);
    }

    [Fact]
    public async Task CreateLeagueAsync_AddsCommissionerMemberWithCommissionerRole()
    {
        var commissionerId = Guid.NewGuid();
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.CreateLeagueAsync("Test League", commissionerId);

        var member = Assert.Single(result.Members);
        Assert.Equal(commissionerId, member.UserId);
        Assert.Equal(LeagueRole.Commissioner, member.Role);
    }

    [Fact]
    public async Task CreateLeagueAsync_GeneratesSlugFromName()
    {
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.CreateLeagueAsync("My Test League", Guid.NewGuid());

        Assert.Equal("my-test-league", result.Slug);
    }

    [Fact]
    public async Task CreateLeagueAsync_SlugRemovesSpecialChars()
    {
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _sut.CreateLeagueAsync("Bob's \"League\"", Guid.NewGuid());

        Assert.Equal("bobs-league", result.Slug);
    }

    [Fact]
    public async Task CreateLeagueAsync_SlugAppendsCounterOnCollision()
    {
        _leagueRepoMock.SetupSequence(r => r.SlugExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true)   // "my-league" exists
            .ReturnsAsync(true)   // "my-league-1" exists
            .ReturnsAsync(false); // "my-league-2" is free

        var result = await _sut.CreateLeagueAsync("My League", Guid.NewGuid());

        Assert.Equal("my-league-2", result.Slug);
    }

    [Fact]
    public async Task CreateLeagueAsync_CallsRepositoryAddAndSave()
    {
        _leagueRepoMock.Setup(r => r.SlugExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        await _sut.CreateLeagueAsync("Test League", Guid.NewGuid());

        _leagueRepoMock.Verify(r => r.AddAsync(It.IsAny<League>()), Times.Once);
        _leagueRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task JoinLeagueAsync_ReturnsManagerMember()
    {
        var leagueId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var league = new League { Id = leagueId, Name = "Test", Slug = "test", CommissionerId = Guid.NewGuid() };
        _leagueRepoMock.Setup(r => r.GetByIdAsync(leagueId)).ReturnsAsync(league);

        var result = await _sut.JoinLeagueAsync(leagueId, userId);

        Assert.Equal(userId, result.UserId);
        Assert.Equal(leagueId, result.LeagueId);
        Assert.Equal(LeagueRole.Manager, result.Role);
    }

    [Fact]
    public async Task JoinLeagueAsync_AddsMemberToLeague()
    {
        var leagueId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var league = new League { Id = leagueId, Name = "Test", Slug = "test", CommissionerId = Guid.NewGuid() };
        _leagueRepoMock.Setup(r => r.GetByIdAsync(leagueId)).ReturnsAsync(league);

        await _sut.JoinLeagueAsync(leagueId, userId);

        Assert.Single(league.Members);
    }

    [Fact]
    public async Task JoinLeagueAsync_CallsSaveChanges()
    {
        var leagueId = Guid.NewGuid();
        var league = new League { Id = leagueId, Name = "Test", Slug = "test", CommissionerId = Guid.NewGuid() };
        _leagueRepoMock.Setup(r => r.GetByIdAsync(leagueId)).ReturnsAsync(league);

        await _sut.JoinLeagueAsync(leagueId, Guid.NewGuid());

        _leagueRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task JoinLeagueAsync_ThrowsWhenLeagueNotFound()
    {
        _leagueRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((League?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.JoinLeagueAsync(Guid.NewGuid(), Guid.NewGuid()));
    }
}
