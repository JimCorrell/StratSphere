using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using StratSphere.Web.Models.ViewModels.League;
using System.Security.Claims;

namespace StratSphere.Web.Controllers;

[Authorize]
public class LeagueController(
    ILeagueRepository leagueRepo,
    IStandingsRepository standingsRepo,
    UserManager<ApplicationUser> userManager,
    LeagueService leagueService) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /league
    public async Task<IActionResult> Index()
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        var isAdmin = user?.IsAdmin == true;

        IEnumerable<League> leagues = isAdmin
            ? await leagueRepo.GetAllAsync()
            : await leagueRepo.GetByUserIdAsync(CurrentUserId);

        var model = new LeagueIndexViewModel
        {
            IsAdmin = isAdmin,
            Leagues = leagues.Select(l => new LeagueIndexViewModel.LeagueSummary
            {
                Name = l.Name,
                Slug = l.Slug,
                Abbreviation = l.Abbreviation,
                Status = l.Status.ToString(),
                MemberCount = l.Members.Count,
                Role = l.Members.FirstOrDefault(m => m.UserId == CurrentUserId)?.Role.ToString() ?? "",
                IsArchived = l.ArchivedAt.HasValue
            })
        };

        return View(model);
    }

    // GET /league/create
    public async Task<IActionResult> Create()
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        return View(new CreateLeagueViewModel
        {
            ShowPhoneField = string.IsNullOrWhiteSpace(user?.PhoneNumber)
        });
    }

    // POST /league/create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLeagueViewModel model)
    {
        // Normalize abbreviation before validation
        model.Abbreviation = model.Abbreviation?.Trim().ToUpperInvariant() ?? string.Empty;

        if (!ModelState.IsValid)
        {
            var currentUser = await userManager.FindByIdAsync(CurrentUserId.ToString());
            model.ShowPhoneField = string.IsNullOrWhiteSpace(currentUser?.PhoneNumber);
            return View(model);
        }

        if (await leagueRepo.AbbreviationExistsAsync(model.Abbreviation))
        {
            ModelState.AddModelError(nameof(model.Abbreviation), "That abbreviation is already taken.");
            var currentUser = await userManager.FindByIdAsync(CurrentUserId.ToString());
            model.ShowPhoneField = string.IsNullOrWhiteSpace(currentUser?.PhoneNumber);
            return View(model);
        }

        // Save phone number if provided and user doesn't have one yet
        if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
            if (user is not null && string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                user.PhoneNumber = model.PhoneNumber;
                await userManager.UpdateAsync(user);
            }
        }

        var league = await leagueService.CreateLeagueAsync(model.Name, model.Abbreviation, CurrentUserId);
        return Redirect($"/league/{league.Abbreviation}");
    }

    // GET /league/join
    public IActionResult Join() => View(new JoinLeagueViewModel());

    // POST /league/join
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(JoinLeagueViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var league = await leagueRepo.GetBySlugAsync(model.Slug.Trim().ToLower());

        if (league is null)
        {
            ModelState.AddModelError(nameof(model.Slug), "No league found with that slug.");
            return View(model);
        }

        if (league.Members.Any(m => m.UserId == CurrentUserId))
        {
            return Redirect($"/league/{league.Abbreviation}");
        }

        await leagueService.JoinLeagueAsync(league.Id, CurrentUserId);
        return Redirect($"/league/{league.Abbreviation}");
    }

    // GET /league/{slug}
    [LeagueMember]
    public async Task<IActionResult> Detail(Guid? seasonId = null)
    {
        var league = HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League;
        if (league is null) return NotFound();

        var userId = CurrentUserId;
        var user = await userManager.FindByIdAsync(userId.ToString());
        var isAdmin = user?.IsAdmin == true;
        var isCommissioner = isAdmin || league.CommissionerId == userId;

        // Resolve selected season: explicit → active → most recent
        var orderedSeasons = league.Seasons
            .OrderByDescending(s => s.CardYear)
            .ThenBy(s => s.Name)
            .ToList();

        Season? selectedSeason = null;
        if (seasonId.HasValue)
            selectedSeason = orderedSeasons.FirstOrDefault(s => s.Id == seasonId.Value);
        selectedSeason ??= orderedSeasons.FirstOrDefault(s => s.Status == SeasonStatus.Active);
        selectedSeason ??= orderedSeasons.FirstOrDefault();

        // Fetch standings for selected season
        var standingRows = new List<LeagueDetailViewModel.StandingRow>();
        if (selectedSeason is not null)
        {
            var standings = await standingsRepo.GetBySeasonIdAsync(selectedSeason.Id);
            standingRows = standings
                .OrderByDescending(s => s.WinPct)
                .ThenByDescending(s => s.RunDiff)
                .Select(s => new LeagueDetailViewModel.StandingRow
                {
                    TeamName = $"{s.Team.City} {s.Team.Name}",
                    Abbreviation = s.Team.Abbreviation,
                    ManagerName = s.Team.User?.DisplayName ?? "—",
                    Wins = s.Wins,
                    Losses = s.Losses,
                    Ties = s.Ties,
                    WinPct = s.WinPct,
                    RunsScored = s.RunsScored,
                    RunsAllowed = s.RunsAllowed,
                    RunDiff = s.RunDiff
                })
                .ToList();
        }

        var model = new LeagueDetailViewModel
        {
            Id = league.Id,
            Name = league.Name,
            Slug = league.Slug,
            Abbreviation = league.Abbreviation,
            Status = league.Status.ToString(),
            IsCommissioner = isCommissioner,
            IsAdmin = isAdmin,
            IsArchived = league.ArchivedAt.HasValue,
            HasTeam = league.Teams.Any(t => t.UserId == userId),
            UserEmailConfirmed = user?.EmailConfirmed ?? false,
            SelectedSeasonId = selectedSeason?.Id,
            SelectedSeasonCardYear = selectedSeason?.CardYear,
            SelectedSeasonName = selectedSeason?.Name,
            Standings = standingRows,
            AssignableMembers = isCommissioner
                ? league.Members
                    .Where(m => m.UserId != userId && m.User?.IsAdmin != true)
                    .Select(m => new LeagueDetailViewModel.AssignableMember
                    {
                        UserId = m.UserId,
                        DisplayName = m.User.DisplayName
                    })
                    .OrderBy(m => m.DisplayName)
                : [],
            Members = league.Members.Select(m => new LeagueDetailViewModel.MemberRow
            {
                DisplayName = m.User.DisplayName,
                Role = m.Role == LeagueRole.Commissioner ? "Commissioner" : "Manager"
            }).OrderBy(m => m.Role).ThenBy(m => m.DisplayName),
            Teams = league.Teams.Select(t => new LeagueDetailViewModel.TeamRow
            {
                Id = t.Id,
                City = t.City,
                Name = t.Name,
                Abbreviation = t.Abbreviation,
                ManagerName = t.User?.DisplayName,
                IsOwner = t.UserId == userId,
                IsClaimed = t.UserId.HasValue
            }),
            Seasons = orderedSeasons.Select(s => new LeagueDetailViewModel.SeasonRow
            {
                Id = s.Id,
                Name = s.Name,
                CardYear = s.CardYear,
                Status = s.Status.ToString()
            })
        };

        return View(model);
    }

    // POST /league/{leagueAbbr}/assign-commissioner
    [HttpPost, ValidateAntiForgeryToken]
    [LeagueMember(CommissionerOnly = true)]
    public async Task<IActionResult> AssignCommissioner(AssignCommissionerViewModel model)
    {
        var leagueAbbr = RouteData.Values["leagueAbbr"] as string ?? "";
        var league = await leagueRepo.GetByAbbreviationAsync(leagueAbbr);
        if (league is null) return NotFound();

        var newCommissioner = league.Members.FirstOrDefault(m => m.UserId == model.NewCommissionerId);
        if (newCommissioner is null)
        {
            TempData["Error"] = "Selected user is not a member of this league.";
            return Redirect($"/league/{league.Abbreviation}");
        }

        var oldCommissioner = league.Members.First(m => m.UserId == league.CommissionerId);
        oldCommissioner.Role = LeagueRole.Manager;
        newCommissioner.Role = LeagueRole.Commissioner;
        league.CommissionerId = model.NewCommissionerId;

        await leagueRepo.SaveChangesAsync();

        TempData["Success"] = $"{newCommissioner.User.DisplayName} is now the commissioner.";
        return Redirect($"/league/{league.Abbreviation}");
    }

    // POST /league/{leagueAbbr}/archive  (admin only)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(string leagueAbbr)
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        if (user?.IsAdmin != true) return Forbid();

        var league = await leagueRepo.GetByAbbreviationAsync(leagueAbbr);
        if (league is null) return NotFound();

        await leagueRepo.ArchiveAsync(league.Id);
        TempData["Success"] = $"League \"{league.Name}\" has been archived.";
        return Redirect($"/league/{leagueAbbr}");
    }

    // POST /league/{leagueAbbr}/unarchive  (admin only)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unarchive(string leagueAbbr)
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        if (user?.IsAdmin != true) return Forbid();

        var league = await leagueRepo.GetByAbbreviationAsync(leagueAbbr);
        if (league is null) return NotFound();

        await leagueRepo.UnarchiveAsync(league.Id);
        TempData["Success"] = $"League \"{league.Name}\" has been restored.";
        return Redirect($"/league/{leagueAbbr}");
    }

    // POST /league/{leagueAbbr}/delete  (admin only)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLeague(string leagueAbbr)
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        if (user?.IsAdmin != true) return Forbid();

        var league = await leagueRepo.GetByAbbreviationAsync(leagueAbbr);
        if (league is null) return NotFound();

        await leagueRepo.DeleteAsync(league.Id);
        TempData["Success"] = $"League \"{league.Name}\" and all its data have been permanently deleted.";
        return Redirect("/league");
    }

    // POST /league/delete-all  (admin only)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllLeagues()
    {
        var user = await userManager.FindByIdAsync(CurrentUserId.ToString());
        if (user?.IsAdmin != true) return Forbid();

        await leagueRepo.DeleteAllAsync();
        TempData["Success"] = "All leagues and related data have been permanently deleted.";
        return Redirect("/league");
    }

    // GET /league/not-member?slug={slug}
    public async Task<IActionResult> NotMember(string slug)
    {
        var league = await leagueRepo.GetBySlugAsync(slug ?? "");

        var model = new NotMemberViewModel
        {
            Slug = slug ?? "",
            LeagueName = league?.Name ?? slug ?? ""
        };

        return View(model);
    }
}
