using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Enums;
using Stratsphere.Core.Interfaces;
using Stratsphere.Core.Services;
using Stratsphere.Web.Filters;
using Stratsphere.Web.Middleware;
using Stratsphere.Web.Models.ViewModels.League;
using System.Security.Claims;

namespace Stratsphere.Web.Controllers;

[Authorize]
public class LeagueController(
    ILeagueRepository leagueRepo,
    LeagueService leagueService) : Controller
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /league
    public async Task<IActionResult> Index()
    {
        var leagues = await leagueRepo.GetByUserIdAsync(CurrentUserId);

        var model = new LeagueIndexViewModel
        {
            Leagues = leagues.Select(l => new LeagueIndexViewModel.LeagueSummary
            {
                Name = l.Name,
                Slug = l.Slug,
                Status = l.Status,
                MemberCount = l.Members.Count,
                Role = l.Members.FirstOrDefault(m => m.UserId == CurrentUserId)?.Role.ToString() ?? ""
            })
        };

        return View(model);
    }

    // GET /league/create
    public IActionResult Create() => View(new CreateLeagueViewModel());

    // POST /league/create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLeagueViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var league = await leagueService.CreateLeagueAsync(model.Name, CurrentUserId);
        return Redirect($"/league/{league.Slug}");
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
            return Redirect($"/league/{league.Slug}");
        }

        await leagueService.JoinLeagueAsync(league.Id, CurrentUserId);
        return Redirect($"/league/{league.Slug}");
    }

    // GET /league/{slug}  (matched by the "league" conventional route)
    [LeagueMember]
    public IActionResult Detail()
    {
        var league = HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League;
        if (league is null) return NotFound();

        var userId = CurrentUserId;

        var model = new LeagueDetailViewModel
        {
            Id = league.Id,
            Name = league.Name,
            Slug = league.Slug,
            Status = league.Status,
            IsCommissioner = league.CommissionerId == userId,
            HasTeam = league.Teams.Any(t => t.UserId == userId),
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
                ManagerName = t.User.DisplayName,
                IsOwner = t.UserId == userId
            })
        };

        return View(model);
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
