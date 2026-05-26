using StratSphere.Core.Interfaces;
using System.Security.Claims;

namespace StratSphere.Web.Middleware;

/// <summary>
/// Resolves the active league from the route slug and attaches it to HttpContext.Items.
/// Also loads the user's full league list for the tenant switcher on every authenticated request.
/// </summary>
public class LeagueContextMiddleware(RequestDelegate next)
{
    public const string LeagueKey        = "ActiveLeague";
    public const string UserLeaguesKey   = "UserLeagues";
    public const string IsCommissionerKey = "IsCommissioner";

    public async Task InvokeAsync(HttpContext context, ILeagueRepository leagueRepo)
    {
        // Resolve active league from leagueAbbr route param
        if (context.Request.RouteValues.TryGetValue("leagueAbbr", out var abbrObj)
            && abbrObj is string abbr && !string.IsNullOrEmpty(abbr))
        {
            var league = await leagueRepo.GetByAbbreviationAsync(abbr);
            if (league is not null)
            {
                context.Items[LeagueKey] = league;

                var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdStr, out var uid))
                    context.Items[IsCommissionerKey] = league.CommissionerId == uid;
            }
        }

        // Load all of the user's leagues for the tenant switcher (authenticated only)
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
                context.Items[UserLeaguesKey] = await leagueRepo.GetByUserIdAsync(userId);
        }

        await next(context);
    }
}
