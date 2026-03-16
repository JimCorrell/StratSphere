using Microsoft.AspNetCore.Http;
using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Web.Middleware;

namespace StratSphere.Tests.Middleware;

public class LeagueContextMiddlewareTests
{
    // ── helpers ───────────────────────────────────────────────────────────────

    private static League MakeLeague(string abbr) =>
        new() { Id = Guid.NewGuid(), Name = "Test", Slug = abbr.ToLower(), Abbreviation = abbr, CommissionerId = Guid.NewGuid(), Status = LeagueStatus.Setup };

    private static (LeagueContextMiddleware middleware, bool[] nextCalled) MakeMiddleware()
    {
        var tracker = new[] { false };
        var middleware = new LeagueContextMiddleware(ctx =>
        {
            tracker[0] = true;
            return Task.CompletedTask;
        });
        return (middleware, tracker);
    }

    // ── no leagueAbbr in route → next called, no league set ──────────────────

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenNoSlugInRoute()
    {
        var (middleware, nextCalled) = MakeMiddleware();
        var httpContext = new DefaultHttpContext();
        // No route values set
        var repoMock = new Mock<ILeagueRepository>();

        await middleware.InvokeAsync(httpContext, repoMock.Object);

        Assert.True(nextCalled[0]);
        Assert.False(httpContext.Items.ContainsKey(LeagueContextMiddleware.LeagueKey));
        repoMock.Verify(r => r.GetByAbbreviationAsync(It.IsAny<string>()), Times.Never);
    }

    // ── empty leagueAbbr → next called, no league set ────────────────────────

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenSlugIsEmpty()
    {
        var (middleware, nextCalled) = MakeMiddleware();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["leagueAbbr"] = string.Empty;
        var repoMock = new Mock<ILeagueRepository>();

        await middleware.InvokeAsync(httpContext, repoMock.Object);

        Assert.True(nextCalled[0]);
        Assert.False(httpContext.Items.ContainsKey(LeagueContextMiddleware.LeagueKey));
        repoMock.Verify(r => r.GetByAbbreviationAsync(It.IsAny<string>()), Times.Never);
    }

    // ── abbreviation found → league stored in Items ──────────────────────────

    [Fact]
    public async Task InvokeAsync_StoresLeagueInItems_WhenSlugMatches()
    {
        var (middleware, nextCalled) = MakeMiddleware();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["leagueAbbr"] = "NL";

        var league = MakeLeague("NL");
        var repoMock = new Mock<ILeagueRepository>();
        repoMock.Setup(r => r.GetByAbbreviationAsync("NL")).ReturnsAsync(league);

        await middleware.InvokeAsync(httpContext, repoMock.Object);

        Assert.True(nextCalled[0]);
        Assert.Same(league, httpContext.Items[LeagueContextMiddleware.LeagueKey]);
    }

    // ── abbreviation not found → next called, no league set ──────────────────

    [Fact]
    public async Task InvokeAsync_DoesNotSetItem_WhenLeagueNotFound()
    {
        var (middleware, nextCalled) = MakeMiddleware();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["leagueAbbr"] = "UNKNOWN";

        var repoMock = new Mock<ILeagueRepository>();
        repoMock.Setup(r => r.GetByAbbreviationAsync("UNKNOWN")).ReturnsAsync((League?)null);

        await middleware.InvokeAsync(httpContext, repoMock.Object);

        Assert.True(nextCalled[0]);
        Assert.False(httpContext.Items.ContainsKey(LeagueContextMiddleware.LeagueKey));
    }

    // ── next always called regardless of outcome ──────────────────────────────

    [Fact]
    public async Task InvokeAsync_AlwaysCallsNext()
    {
        var (middleware, nextCalled) = MakeMiddleware();
        var httpContext = new DefaultHttpContext();
        // No leagueAbbr — verifies that even with no abbr, next is called
        var repoMock = new Mock<ILeagueRepository>();

        await middleware.InvokeAsync(httpContext, repoMock.Object);

        Assert.True(nextCalled[0]);
    }
}
