using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Interfaces;
using Stratsphere.Web.Filters;
using Stratsphere.Web.Middleware;
using Stratsphere.Core.Entities;

namespace Stratsphere.Web.Controllers;

[Authorize]
public class StandingsController(
    IStandingsRepository standingsRepo,
    ILeagueRepository leagueRepo) : Controller
{
    // GET /league/{slug}/standings
    [LeagueMember]
    public async Task<IActionResult> Index(Guid seasonId)
    {
        var league = (League)HttpContext.Items[LeagueContextMiddleware.LeagueKey]!;
        var season = league.Seasons.FirstOrDefault(s => s.Id == seasonId)
                     ?? league.Seasons.OrderByDescending(s => s.StartDate).FirstOrDefault();

        if (season is null) return View("NoSeason");

        var standings = await standingsRepo.GetBySeasonIdAsync(season.Id);
        ViewBag.Season = season;
        ViewBag.League = league;
        return View(standings);
    }
}
