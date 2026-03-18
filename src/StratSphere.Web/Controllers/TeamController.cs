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

    // GET /league/{leagueAbbr}/team/create  — commissioner only
    [LeagueMember(CommissionerOnly = true)]
    public IActionResult Create() => View(new CreateTeamViewModel());

    // POST /league/{leagueAbbr}/team/create  — commissioner only
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> Create(CreateTeamViewModel model)
    {
        var league = ActiveLeague;

        if (!ModelState.IsValid) return View(model);

        var abbreviation = model.Abbreviation.Trim().ToUpper();

        if (await teamRepo.AbbreviationExistsInLeagueAsync(league.Id, abbreviation))
        {
            ModelState.AddModelError(nameof(model.Abbreviation), "That abbreviation is already used by another team in this league.");
            return View(model);
        }

        var team = new Team
        {
            Id = Guid.NewGuid(),
            LeagueId = league.Id,
            UserId = null,
            City = model.City.Trim(),
            Name = model.Name.Trim(),
            Abbreviation = abbreviation
        };

        await teamRepo.AddAsync(team);
        await teamRepo.SaveChangesAsync();

        return Redirect($"/league/{league.Abbreviation}");
    }

    // POST /league/{leagueAbbr}/season/{year}/team/{teamAbbr}/claim
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Claim(int year, string teamAbbr)
    {
        var league = ActiveLeague;
        var userId = CurrentUserId;

        var team = league.Teams.FirstOrDefault(t =>
            t.Abbreviation.ToUpperInvariant() == teamAbbr.ToUpperInvariant());
        if (team is null)
        {
            TempData["Error"] = $"Team \"{teamAbbr.ToUpper()}\" not found in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        // Prevent claiming if user already has a team in this league
        if (league.Teams.Any(t => t.UserId == userId))
            return Redirect($"/league/{league.Abbreviation}");

        await teamRepo.ClaimAsync(team.Id, userId, league.Id);

        return Redirect($"/league/{league.Abbreviation}/season/{year}/team/{teamAbbr.ToUpper()}");
    }

    // GET /league/{leagueAbbr}/season/{year}/team/{teamAbbr}
    public async Task<IActionResult> Detail(int year, string teamAbbr)
    {
        var league = ActiveLeague;
        var userId = CurrentUserId;

        var team = league.Teams.FirstOrDefault(t =>
            t.Abbreviation.ToUpperInvariant() == teamAbbr.ToUpperInvariant());
        if (team is null)
        {
            TempData["Error"] = $"Team \"{teamAbbr.ToUpper()}\" not found in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        var season = league.Seasons.FirstOrDefault(s => s.CardYear == year);
        if (season is null)
        {
            TempData["Error"] = $"No season found for year {year} in this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        var isOwner = team.UserId == userId;
        var isCommissioner = league.CommissionerId == userId;
        var canManage = isOwner || isCommissioner;

        // Build season list for tabs
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

        var selected = seasons.FirstOrDefault(s => s.CardYear == year);

        // Load roster — batch all Lahman lookups to avoid N+1
        var roster = new List<TeamDetailViewModel.RosterRow>();
        if (selected is not null)
        {
            var slots = (await rosterRepo.GetByTeamAndSeasonAsync(team.Id, season.Id)).ToList();
            if (slots.Count > 0)
            {
                var playerIds = slots.Select(s => s.Card.LahmanPlayerId).Distinct().ToList();
                var years     = slots.Select(s => s.Card.CardYear).Distinct().ToList();

                var people        = await lahmanRepo.GetPeopleAsync(playerIds);
                var battingStats  = await lahmanRepo.GetBattingSeasonsAsync(playerIds, years);
                var pitchingStats = await lahmanRepo.GetPitchingSeasonsAsync(playerIds, years);

                foreach (var slot in slots)
                {
                    if (!people.TryGetValue(slot.Card.LahmanPlayerId, out var person)) continue;

                    var isPitcher = slot.Card.Position is "SP" or "RP";
                    string statLine;
                    if (isPitcher)
                    {
                        pitchingStats.TryGetValue((slot.Card.LahmanPlayerId, slot.Card.CardYear), out var p);
                        statLine = p?.ERA.HasValue == true ? $"{p.ERA:F2} ERA" : "";
                    }
                    else
                    {
                        battingStats.TryGetValue((slot.Card.LahmanPlayerId, slot.Card.CardYear), out var b);
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
        }

        var model = new TeamDetailViewModel
        {
            Id = team.Id,
            City = team.City,
            Name = team.Name,
            Abbreviation = team.Abbreviation,
            OwnerName = team.User?.DisplayName,
            LeagueName = league.Name,
            LeagueAbbreviation = league.Abbreviation,
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
