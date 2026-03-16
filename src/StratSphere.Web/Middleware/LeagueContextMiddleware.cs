using StratSphere.Core.Interfaces;

namespace StratSphere.Web.Middleware;

/// <summary>
/// Resolves the active league from the route slug and attaches it to HttpContext.Items.
/// This means controllers don't need to re-query the league on every request.
/// </summary>
public class LeagueContextMiddleware(RequestDelegate next)
{
    public const string LeagueKey = "ActiveLeague";

    public async Task InvokeAsync(HttpContext context, ILeagueRepository leagueRepo)
    {
        if (context.Request.RouteValues.TryGetValue("leagueAbbr", out var abbrObj)
            && abbrObj is string abbr && !string.IsNullOrEmpty(abbr))
        {
            var league = await leagueRepo.GetByAbbreviationAsync(abbr);
            if (league is not null)
                context.Items[LeagueKey] = league;
        }

        await next(context);
    }
}
