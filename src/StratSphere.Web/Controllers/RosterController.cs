using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using StratSphere.Web.Models.ViewModels.Roster;
using System.Security.Claims;

namespace StratSphere.Web.Controllers;

[Authorize]
[LeagueMember]
public class RosterController(
    RosterService rosterSvc,
    ILahmanRepository lahmanRepo) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private League ActiveLeague =>
        (HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League)!;

    // GET /league/{leagueAbbr}/Roster/SearchCards?teamId=&year=&q=
    public async Task<IActionResult> SearchCards(Guid teamId, int year, string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Json(Array.Empty<object>());

        var batters = await lahmanRepo.SearchCardsAsync(q, year, pitchersOnly: false, limit: 8);
        var pitchers = await lahmanRepo.SearchCardsAsync(q, year, pitchersOnly: true, limit: 5);

        var results = batters
            .Select(r => new
            {
                lahmanPlayerId = r.Person.PlayerId,
                name = r.Person.FullName,
                position = r.PrimaryPosition,
                cardYear = year
            })
            .Concat(pitchers.Select(r => new
            {
                lahmanPlayerId = r.Person.PlayerId,
                name = r.Person.FullName,
                position = r.PrimaryPosition,
                cardYear = year
            }))
            .OrderBy(r => r.name)
            .ToList();

        return Json(results);
    }

    // POST /league/{leagueAbbr}/Roster/AddPlayer
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPlayer(AddPlayerViewModel model)
    {
        var league = ActiveLeague;
        var team = league.Teams.FirstOrDefault(t => t.Id == model.TeamId);

        if (team is null || team.LeagueId != league.Id)
        {
            TempData["Error"] = "Team not found in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        if (!CanManage(team, league))
            return Forbid();

        try
        {
            await rosterSvc.AddCardToRosterAsync(
                model.TeamId, model.SeasonId,
                model.LahmanPlayerId, model.CardYear, model.Position);
        }
        catch (InvalidOperationException)
        {
            // Card already rostered — ignore silently, redirect back
        }

        return Redirect($"/league/{league.Abbreviation}/season/{model.CardYear}/team/{team.Abbreviation}");
    }

    // POST /league/{leagueAbbr}/Roster/Drop
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Drop(Guid slotId, Guid teamId, Guid seasonId, int cardYear, string teamAbbr)
    {
        var league = ActiveLeague;
        var team = league.Teams.FirstOrDefault(t => t.Id == teamId);

        if (team is null || team.LeagueId != league.Id)
        {
            TempData["Error"] = "Team not found in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        if (!CanManage(team, league))
            return Forbid();

        await rosterSvc.DropCardFromRosterAsync(slotId, teamId);

        return Redirect($"/league/{league.Abbreviation}/season/{cardYear}/team/{teamAbbr}");
    }

    private bool CanManage(Team team, League league) =>
        team.UserId == CurrentUserId || league.CommissionerId == CurrentUserId;
}
