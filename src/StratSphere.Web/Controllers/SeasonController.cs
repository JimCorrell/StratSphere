using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
public class SeasonController(
    ISeasonRepository seasonRepo,
    IRosterRepository rosterRepo,
    UserManager<ApplicationUser> userManager) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private League ActiveLeague =>
        (HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League)!;

    // GET /league/{leagueAbbr}/season/create
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Create()
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        if (user is null || !user.EmailConfirmed)
        {
            TempData["Error"] = "You must confirm your email before creating a season.";
            return Redirect($"/league/{ActiveLeague.Abbreviation}");
        }

        return View(new CreateSeasonViewModel { LeagueAbbreviation = ActiveLeague.Abbreviation });
    }

    // POST /league/{leagueAbbr}/season/create
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Create(CreateSeasonViewModel model)
    {
        var league = ActiveLeague;

        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        if (user is null || !user.EmailConfirmed)
        {
            TempData["Error"] = "You must confirm your email before creating a season.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        model.LeagueAbbreviation = league.Abbreviation;

        if (!ModelState.IsValid) return View(model);

        // Enforce one season per card year per league
        var existing = await seasonRepo.GetByLeagueAndYearAsync(league.Id, model.CardYear);
        if (existing is not null)
        {
            ModelState.AddModelError(nameof(model.CardYear), $"A season for {model.CardYear} already exists in this league.");
            return View(model);
        }

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

        return Redirect($"/league/{league.Abbreviation}/season/{season.CardYear}");
    }

    // GET /league/{leagueAbbr}/season/{year}
    public async Task<IActionResult> Detail(int year)
    {
        var league = ActiveLeague;
        var season = await seasonRepo.GetByLeagueAndYearAsync(league.Id, year);

        if (season is null)
        {
            TempData["Error"] = $"No season found for year {year} in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        // Build team roster rows
        var teamRows = new List<SeasonDetailViewModel.TeamRosterRow>();
        foreach (var team in league.Teams)
        {
            var slots = await rosterRepo.GetByTeamAndSeasonAsync(team.Id, season.Id);
            teamRows.Add(new SeasonDetailViewModel.TeamRosterRow
            {
                TeamId = team.Id,
                City = team.City,
                Name = team.Name,
                Abbreviation = team.Abbreviation,
                ManagerName = team.User?.DisplayName,
                IsClaimed = team.UserId.HasValue,
                RosterCount = slots.Count(s => s.DroppedAt == null)
            });
        }

        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        var isCommissioner = user?.IsAdmin == true || league.CommissionerId == CurrentUserId;

        var model = new SeasonDetailViewModel
        {
            Id = season.Id,
            Name = season.Name,
            CardYear = season.CardYear,
            Status = season.Status.ToString(),
            LeagueName = league.Name,
            LeagueAbbreviation = league.Abbreviation,
            IsCommissioner = isCommissioner,
            Teams = teamRows.OrderBy(t => t.City).ThenBy(t => t.Name)
        };

        return View(model);
    }

    // POST /league/{leagueAbbr}/season/{year}/activate
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Activate(int year)
    {
        var league = ActiveLeague;
        var season = await seasonRepo.GetByLeagueAndYearAsync(league.Id, year);
        if (season is null)
        {
            TempData["Error"] = $"No season found for year {year} in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        if (season.Status != SeasonStatus.Setup)
        {
            TempData["Error"] = "Season can only be activated from Setup status.";
            return Redirect($"/league/{league.Abbreviation}/season/{year}");
        }

        await seasonRepo.UpdateStatusAsync(season.Id, SeasonStatus.Active);
        TempData["Success"] = $"Season \"{season.Name}\" is now active.";
        return Redirect($"/league/{league.Abbreviation}/season/{year}");
    }

    // POST /league/{leagueAbbr}/season/{year}/complete
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Complete(int year)
    {
        var league = ActiveLeague;
        var season = await seasonRepo.GetByLeagueAndYearAsync(league.Id, year);
        if (season is null)
        {
            TempData["Error"] = $"No season found for year {year} in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        if (season.Status != SeasonStatus.Active)
        {
            TempData["Error"] = "Season can only be completed from Active status.";
            return Redirect($"/league/{league.Abbreviation}/season/{year}");
        }

        await seasonRepo.UpdateStatusAsync(season.Id, SeasonStatus.Completed);
        TempData["Success"] = $"Season \"{season.Name}\" has been completed.";
        return Redirect($"/league/{league.Abbreviation}/season/{year}");
    }
}
