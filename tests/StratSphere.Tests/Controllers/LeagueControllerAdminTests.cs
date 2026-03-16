using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Web.Controllers;
using System.Security.Claims;

namespace StratSphere.Tests.Controllers;

public class LeagueControllerAdminTests
{
    private readonly Mock<ILeagueRepository> _leagueRepo;
    private readonly Mock<IStandingsRepository> _standingsRepo;
    private readonly Mock<UserManager<ApplicationUser>> _userManager;

    public LeagueControllerAdminTests()
    {
        _leagueRepo = new Mock<ILeagueRepository>();
        _standingsRepo = new Mock<IStandingsRepository>();
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    private LeagueController MakeController(Guid userId)
    {
        var leagueService = new LeagueService(_leagueRepo.Object);
        var controller = new LeagueController(
            _leagueRepo.Object, _standingsRepo.Object, _userManager.Object, leagueService);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "Test"))
            }
        };
        controller.TempData = new TempDataDictionary(
            controller.ControllerContext.HttpContext,
            Mock.Of<ITempDataProvider>());

        return controller;
    }

    private static ApplicationUser MakeUser(bool isAdmin) => new()
    {
        Id = Guid.NewGuid(),
        UserName = "test@test.com",
        IsAdmin = isAdmin
    };

    private static League MakeLeague(string abbr = "TL") => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test League",
        Slug = abbr.ToLower(),
        Abbreviation = abbr,
        CommissionerId = Guid.NewGuid(),
        Status = LeagueStatus.Setup
    };

    // ── Archive ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Archive_ReturnsForbid_WhenUserIsNotAdmin()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: false));

        var result = await MakeController(userId).Archive("TL");

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Archive_ReturnsNotFound_WhenLeagueDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));
        _leagueRepo.Setup(r => r.GetByAbbreviationAsync("MISSING")).ReturnsAsync((League?)null);

        var result = await MakeController(userId).Archive("MISSING");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Archive_CallsArchiveAsync_AndRedirects_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        var league = MakeLeague("NL");
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));
        _leagueRepo.Setup(r => r.GetByAbbreviationAsync("NL")).ReturnsAsync(league);

        var result = await MakeController(userId).Archive("NL");

        _leagueRepo.Verify(r => r.ArchiveAsync(league.Id), Times.Once);
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/league/NL", redirect.Url);
    }

    // ── Unarchive ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Unarchive_ReturnsForbid_WhenUserIsNotAdmin()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: false));

        var result = await MakeController(userId).Unarchive("TL");

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task Unarchive_ReturnsNotFound_WhenLeagueDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));
        _leagueRepo.Setup(r => r.GetByAbbreviationAsync("MISSING")).ReturnsAsync((League?)null);

        var result = await MakeController(userId).Unarchive("MISSING");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Unarchive_CallsUnarchiveAsync_AndRedirects_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        var league = MakeLeague("NL");
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));
        _leagueRepo.Setup(r => r.GetByAbbreviationAsync("NL")).ReturnsAsync(league);

        var result = await MakeController(userId).Unarchive("NL");

        _leagueRepo.Verify(r => r.UnarchiveAsync(league.Id), Times.Once);
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/league/NL", redirect.Url);
    }

    // ── DeleteLeague ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteLeague_ReturnsForbid_WhenUserIsNotAdmin()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: false));

        var result = await MakeController(userId).DeleteLeague("TL");

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteLeague_ReturnsNotFound_WhenLeagueDoesNotExist()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));
        _leagueRepo.Setup(r => r.GetByAbbreviationAsync("MISSING")).ReturnsAsync((League?)null);

        var result = await MakeController(userId).DeleteLeague("MISSING");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteLeague_CallsDeleteAsync_AndRedirectsToIndex_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        var league = MakeLeague("NL");
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));
        _leagueRepo.Setup(r => r.GetByAbbreviationAsync("NL")).ReturnsAsync(league);

        var result = await MakeController(userId).DeleteLeague("NL");

        _leagueRepo.Verify(r => r.DeleteAsync(league.Id), Times.Once);
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/league", redirect.Url);
    }

    // ── DeleteAllLeagues ──────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAllLeagues_ReturnsForbid_WhenUserIsNotAdmin()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: false));

        var result = await MakeController(userId).DeleteAllLeagues();

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task DeleteAllLeagues_CallsDeleteAllAsync_AndRedirectsToIndex_WhenAdmin()
    {
        var userId = Guid.NewGuid();
        _userManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(MakeUser(isAdmin: true));

        var result = await MakeController(userId).DeleteAllLeagues();

        _leagueRepo.Verify(r => r.DeleteAllAsync(), Times.Once);
        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/league", redirect.Url);
    }
}
