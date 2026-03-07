using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using StratSphere.Web.Models.ViewModels.Season;
using System.Security.Claims;

namespace StratSphere.Web.Controllers;

[Authorize]
[LeagueMember]
public class SeasonController(ISeasonRepository seasonRepo) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private League ActiveLeague =>
        (HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League)!;

    // GET /league/{slug}/Season/Create
    [LeagueMember(CommissionerOnly = true)]
    public IActionResult Create() => View(new CreateSeasonViewModel());

    // POST /league/{slug}/Season/Create
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Create(CreateSeasonViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var league = ActiveLeague;
        var season = new Season
        {
            Id = Guid.NewGuid(),
            LeagueId = league.Id,
            Name = model.Name.Trim(),
            CardYear = model.CardYear,
            Status = SeasonStatus.Setup
        };

        await seasonRepo.AddAsync(season);
        await seasonRepo.SaveChangesAsync();

        return Redirect($"/league/{league.Slug}/season/{season.Id}");
    }

    // GET /league/{slug}/season/{id:guid}
    public async Task<IActionResult> Detail(Guid id)
    {
        var league = ActiveLeague;
        var season = await seasonRepo.GetByIdAsync(id);

        if (season is null || season.LeagueId != league.Id) return NotFound();

        var model = new SeasonDetailViewModel
        {
            Id = season.Id,
            Name = season.Name,
            CardYear = season.CardYear,
            Status = season.Status.ToString(),
            LeagueName = league.Name,
            LeagueSlug = league.Slug,
            IsCommissioner = league.CommissionerId == CurrentUserId
        };

        return View(model);
    }
}
