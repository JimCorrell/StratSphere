using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Web.Middleware;
using System.Security.Claims;

namespace StratSphere.Web.Filters;

/// <summary>
/// Action filter that enforces league membership for league-scoped routes.
/// Apply [LeagueMember] to require any membership, [LeagueMember(CommissionerOnly = true)]
/// to restrict to commissioners.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class LeagueMemberAttribute : Attribute, IAsyncActionFilter
{
    public bool CommissionerOnly { get; set; } = false;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var league = httpContext.Items[LeagueContextMiddleware.LeagueKey] as League;

        if (league is null)
        {
            context.Result = new NotFoundResult();
            return;
        }

        var userIdStr = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        // Super admin bypasses all league membership and commissioner checks.
        var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user?.IsAdmin == true)
        {
            await next();
            return;
        }

        var membership = league.Members.FirstOrDefault(m => m.UserId == userId);
        if (membership is null)
        {
            context.Result = new RedirectResult($"/league/not-member?slug={league.Slug}");
            return;
        }

        if (CommissionerOnly && membership.Role != LeagueRole.Commissioner)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
