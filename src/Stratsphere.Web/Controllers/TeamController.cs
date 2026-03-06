using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;
using Stratsphere.Web.Filters;
using Stratsphere.Web.Middleware;
using Stratsphere.Web.Models.ViewModels.Team;
using System.Security.Claims;

namespace Stratsphere.Web.Controllers;

[Authorize]
[LeagueMember]
public class TeamController(ITeamRepository teamRepo) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private League ActiveLeague =>
        (HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League)!;

    // GET /league/{slug}/team/create
    public IActionResult Create()
    {
        var league = ActiveLeague;

        if (league.Teams.Any(t => t.UserId == CurrentUserId))
            return Redirect($"/league/{league.Slug}");

        return View(new CreateTeamViewModel());
    }

    // POST /league/{slug}/team/create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTeamViewModel model)
    {
        var league = ActiveLeague;

        if (league.Teams.Any(t => t.UserId == CurrentUserId))
            return Redirect($"/league/{league.Slug}");

        if (!ModelState.IsValid) return View(model);

        var team = new Team
        {
            Id = Guid.NewGuid(),
            LeagueId = league.Id,
            UserId = CurrentUserId,
            City = model.City.Trim(),
            Name = model.Name.Trim(),
            Abbreviation = model.Abbreviation.Trim().ToUpper()
        };

        await teamRepo.AddAsync(team);
        await teamRepo.SaveChangesAsync();

        return Redirect($"/league/{league.Slug}/team/{team.Id}");
    }

    // GET /league/{slug}/team/{id}
    public async Task<IActionResult> Detail(Guid id)
    {
        var league = ActiveLeague;
        var team = await teamRepo.GetByIdAsync(id);

        if (team is null || team.LeagueId != league.Id)
            return NotFound();

        var model = new TeamDetailViewModel
        {
            Id = team.Id,
            City = team.City,
            Name = team.Name,
            Abbreviation = team.Abbreviation,
            LeagueName = league.Name,
            LeagueSlug = league.Slug,
            IsOwner = team.UserId == CurrentUserId
        };

        return View(model);
    }
}
