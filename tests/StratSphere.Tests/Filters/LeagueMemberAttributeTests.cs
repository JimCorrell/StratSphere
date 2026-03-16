using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using System.Security.Claims;

namespace StratSphere.Tests.Filters;

public class LeagueMemberAttributeTests
{
    // ── helpers ───────────────────────────────────────────────────────────────

    private static (ActionExecutingContext context, DefaultHttpContext httpContext) MakeContext(
        Mock<UserManager<ApplicationUser>>? userManagerMock = null)
    {
        var services = new ServiceCollection();
        if (userManagerMock is not null)
            services.AddSingleton(userManagerMock.Object);
        else
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var um = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            um.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
              .ReturnsAsync((ApplicationUser?)null);
            services.AddSingleton(um.Object);
        }

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?>(),
            controller: null!);
        return (executingContext, httpContext);
    }

    private static League MakeLeague(Guid? id = null) =>
        new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Test",
            Slug = "test",
            Abbreviation = "TL",
            CommissionerId = Guid.NewGuid(),
            Status = LeagueStatus.Setup
        };

    private static ActionExecutionDelegate MakeDelegate(Action? onCall = null) =>
        () =>
        {
            onCall?.Invoke();
            return Task.FromResult<ActionExecutedContext>(null!);
        };

    private static ClaimsPrincipal MakeUser(Guid userId) =>
        new(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, authenticationType: "Test"));

    private static Mock<UserManager<ApplicationUser>> MakeUserManager(ApplicationUser? user = null)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var um = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        um.Setup(m => m.FindByIdAsync(It.IsAny<string>()))
          .ReturnsAsync(user);
        return um;
    }

    // ── no league in context → 404 ────────────────────────────────────────────

    [Fact]
    public async Task OnActionExecution_Returns404_WhenNoLeagueInContext()
    {
        var (ctx, _) = MakeContext();
        // Items["ActiveLeague"] not set

        await new LeagueMemberAttribute().OnActionExecutionAsync(ctx, MakeDelegate());

        Assert.IsType<NotFoundResult>(ctx.Result);
    }

    // ── unauthenticated user → redirect to login ──────────────────────────────

    [Fact]
    public async Task OnActionExecution_RedirectsToLogin_WhenNoUserClaim()
    {
        var (ctx, httpContext) = MakeContext();
        httpContext.Items[LeagueContextMiddleware.LeagueKey] = MakeLeague();
        // httpContext.User has no NameIdentifier claim (default)

        await new LeagueMemberAttribute().OnActionExecutionAsync(ctx, MakeDelegate());

        var redirect = Assert.IsType<RedirectToActionResult>(ctx.Result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Account", redirect.ControllerName);
    }

    // ── non-member → redirect to not-member ──────────────────────────────────

    [Fact]
    public async Task OnActionExecution_RedirectsToNotMember_WhenUserIsNotMember()
    {
        var userId = Guid.NewGuid();
        var um = MakeUserManager(new ApplicationUser { Id = userId, IsAdmin = false });
        var (ctx, httpContext) = MakeContext(um);
        var league = MakeLeague();
        httpContext.Items[LeagueContextMiddleware.LeagueKey] = league;
        httpContext.User = MakeUser(userId); // not in league.Members

        await new LeagueMemberAttribute().OnActionExecutionAsync(ctx, MakeDelegate());

        var redirect = Assert.IsType<RedirectResult>(ctx.Result);
        Assert.Contains("/league/not-member", redirect.Url);
        Assert.Contains(league.Slug, redirect.Url);
    }

    // ── member, CommissionerOnly=false → next called ──────────────────────────

    [Fact]
    public async Task OnActionExecution_CallsNext_WhenUserIsMember()
    {
        var userId = Guid.NewGuid();
        var um = MakeUserManager(new ApplicationUser { Id = userId, IsAdmin = false });
        var (ctx, httpContext) = MakeContext(um);
        var league = MakeLeague();
        league.Members.Add(new LeagueMember { LeagueId = league.Id, UserId = userId, Role = LeagueRole.Manager });
        httpContext.Items[LeagueContextMiddleware.LeagueKey] = league;
        httpContext.User = MakeUser(userId);

        var nextCalled = false;
        await new LeagueMemberAttribute().OnActionExecutionAsync(ctx, MakeDelegate(() => nextCalled = true));

        Assert.True(nextCalled);
        Assert.Null(ctx.Result);
    }

    // ── member (manager), CommissionerOnly=true → 403 ────────────────────────

    [Fact]
    public async Task OnActionExecution_Returns403_WhenManagerOnCommissionerOnlyRoute()
    {
        var userId = Guid.NewGuid();
        var um = MakeUserManager(new ApplicationUser { Id = userId, IsAdmin = false });
        var (ctx, httpContext) = MakeContext(um);
        var league = MakeLeague();
        league.Members.Add(new LeagueMember { LeagueId = league.Id, UserId = userId, Role = LeagueRole.Manager });
        httpContext.Items[LeagueContextMiddleware.LeagueKey] = league;
        httpContext.User = MakeUser(userId);

        await new LeagueMemberAttribute { CommissionerOnly = true }
            .OnActionExecutionAsync(ctx, MakeDelegate());

        Assert.IsType<ForbidResult>(ctx.Result);
    }

    // ── commissioner, CommissionerOnly=true → next called ────────────────────

    [Fact]
    public async Task OnActionExecution_CallsNext_WhenCommissionerOnCommissionerOnlyRoute()
    {
        var userId = Guid.NewGuid();
        var um = MakeUserManager(new ApplicationUser { Id = userId, IsAdmin = false });
        var (ctx, httpContext) = MakeContext(um);
        var league = MakeLeague();
        league.Members.Add(new LeagueMember { LeagueId = league.Id, UserId = userId, Role = LeagueRole.Commissioner });
        httpContext.Items[LeagueContextMiddleware.LeagueKey] = league;
        httpContext.User = MakeUser(userId);

        var nextCalled = false;
        await new LeagueMemberAttribute { CommissionerOnly = true }
            .OnActionExecutionAsync(ctx, MakeDelegate(() => nextCalled = true));

        Assert.True(nextCalled);
        Assert.Null(ctx.Result);
    }
}
