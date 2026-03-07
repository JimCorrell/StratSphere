using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using StratSphere.Web.Models.ViewModels.Team;
using System.Security.Claims;

namespace StratSphere.Web.Controllers;

[Authorize]
[LeagueMember]
public class TeamController(
    ITeamRepository teamRepo,
    IRosterRepository rosterRepo,
    ILahmanRepository lahmanRepo) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private League ActiveLeague =>
        (HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League)!;

    // GET /league/{slug}/team/create  — commissioner only
    [LeagueMember(CommissionerOnly = true)]
    public IActionResult Create() => View(new CreateTeamViewModel());

    // POST /league/{slug}/team/create  — commissioner only
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Create(CreateTeamViewModel model)
    {
        var league = ActiveLeague;

        if (!ModelState.IsValid) return View(model);

        var team = new Team
        {
            Id = Guid.NewGuid(),
            LeagueId = league.Id,
            UserId = null,
            City = model.City.Trim(),
            Name = model.Name.Trim(),
            Abbreviation = model.Abbreviation.Trim().ToUpper()
        };

        await teamRepo.AddAsync(team);
        await teamRepo.SaveChangesAsync();

        return Redirect($"/league/{league.Slug}/team/{team.Id}");
    }

    // POST /league/{slug}/team/{id}/claim
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Claim(Guid id)
    {
        var league = ActiveLeague;
        var userId = CurrentUserId;

        // Prevent claiming if user already has a team in this league
        if (league.Teams.Any(t => t.UserId == userId))
            return Redirect($"/league/{league.Slug}");

        await teamRepo.ClaimAsync(id, userId, league.Id);

        return Redirect($"/league/{league.Slug}/team/{id}");
    }

    // GET /league/{slug}/team/{id}?seasonId=
    public async Task<IActionResult> Detail(Guid id, Guid? seasonId)
    {
        var league = ActiveLeague;
        var userId = CurrentUserId;
        var team = await teamRepo.GetByIdAsync(id);

        if (team is null || team.LeagueId != league.Id)
            return NotFound();

        var isOwner = team.UserId == userId;
        var isCommissioner = league.CommissionerId == userId;
        var canManage = isOwner || isCommissioner;

        // Build season list from the active league (already loaded by middleware)
        var seasons = league.Seasons
            .OrderByDescending(s => s.CardYear)
            .ThenBy(s => s.Name)
            .Select(s => new TeamDetailViewModel.SeasonOption
            {
                Id = s.Id,
                Name = s.Name,
                CardYear = s.CardYear
            })
            .ToList();

        // Default to first season if none specified
        var selected = seasons.FirstOrDefault(s => s.Id == seasonId)
            ?? seasons.FirstOrDefault();

        // Load roster for selected season
        var roster = new List<TeamDetailViewModel.RosterRow>();
        if (selected is not null)
        {
            var slots = await rosterRepo.GetByTeamAndSeasonAsync(team.Id, selected.Id);
            foreach (var slot in slots)
            {
                var person = await lahmanRepo.GetPersonAsync(slot.Card.LahmanPlayerId);
                if (person is null) continue;

                var isPitcher = slot.Card.Position is "SP" or "RP";
                string statLine;
                if (isPitcher)
                {
                    var p = await lahmanRepo.GetPitchingSeasonAsync(slot.Card.LahmanPlayerId, slot.Card.CardYear);
                    statLine = p?.ERA.HasValue == true ? $"{p.ERA:F2} ERA" : "";
                }
                else
                {
                    var b = await lahmanRepo.GetBattingSeasonAsync(slot.Card.LahmanPlayerId, slot.Card.CardYear);
                    statLine = b?.BA.HasValue == true ? $".{(int)(b.BA.Value * 1000):D3}" : "";
                }

                roster.Add(new TeamDetailViewModel.RosterRow
                {
                    SlotId = slot.Id,
                    PlayerName = person.FullName,
                    Position = slot.Card.Position,
                    CardYear = slot.Card.CardYear,
                    StatLine = statLine
                });
            }
        }

        var model = new TeamDetailViewModel
        {
            Id = team.Id,
            City = team.City,
            Name = team.Name,
            Abbreviation = team.Abbreviation,
            OwnerName = team.User?.DisplayName,
            LeagueName = league.Name,
            LeagueSlug = league.Slug,
            IsOwner = isOwner,
            IsClaimed = team.UserId.HasValue,
            CanManage = canManage,
            Seasons = seasons,
            SelectedSeason = selected,
            Roster = roster
        };

        return View(model);
    }
}
