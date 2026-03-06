using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Interfaces;
using Stratsphere.Core.Services;
using Stratsphere.Web.Filters;
using Stratsphere.Web.Middleware;
using Stratsphere.Core.Entities;

namespace Stratsphere.Web.Controllers;

[Authorize]
public class RosterController(
    IRosterRepository rosterRepo,
    ITeamRepository teamRepo,
    RosterService rosterService,
    PlayerCardService cardService,
    ILahmanRepository lahmanRepo) : Controller
{
    // GET /league/{slug}/roster/{teamId}
    [LeagueMember]
    public async Task<IActionResult> Index(Guid teamId, Guid seasonId)
    {
        var league = (League)HttpContext.Items[LeagueContextMiddleware.LeagueKey]!;
        var team = await teamRepo.GetByIdAsync(teamId);
        if (team is null || team.LeagueId != league.Id) return NotFound();

        var slots = await rosterRepo.GetByTeamAndSeasonAsync(teamId, seasonId);
        ViewBag.Team = team;
        ViewBag.SeasonId = seasonId;
        return View(slots);
    }

    // POST /league/{slug}/roster/{teamId}/add
    [HttpPost, ValidateAntiForgeryToken, LeagueMember]
    public async Task<IActionResult> Add(
        Guid teamId, Guid seasonId,
        string lahmanPlayerId, int cardYear, string position)
    {
        try
        {
            await rosterService.AddCardToRosterAsync(teamId, seasonId, lahmanPlayerId, cardYear, position);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { teamId, seasonId });
    }

    // POST /league/{slug}/roster/{teamId}/remove/{slotId}
    [HttpPost, ValidateAntiForgeryToken, LeagueMember]
    public async Task<IActionResult> Remove(Guid teamId, Guid slotId, Guid seasonId)
    {
        await rosterService.RemoveCardFromRosterAsync(slotId);
        return RedirectToAction(nameof(Index), new { teamId, seasonId });
    }
}
