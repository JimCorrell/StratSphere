using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Interfaces;
using Stratsphere.Core.Services;
using Stratsphere.Web.Filters;
using Stratsphere.Web.Middleware;
using System.Security.Claims;

namespace Stratsphere.Web.Controllers;

[Authorize]
public class LeagueController(
    ILeagueRepository leagueRepo,
    LeagueService leagueService) : Controller
{
    // GET /league
    public async Task<IActionResult> Index()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var leagues = await leagueRepo.GetByUserIdAsync(userId);
        return View(leagues);
    }

    // GET /league/create
    public IActionResult Create() => View();

    // POST /league/create
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("", "League name is required.");
            return View();
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var league = await leagueService.CreateLeagueAsync(name, userId);
        return RedirectToAction(nameof(Detail), new { slug = league.Slug });
    }

    // GET /league/{slug}
    [LeagueMember]
    public IActionResult Detail()
    {
        var league = HttpContext.Items[LeagueContextMiddleware.LeagueKey];
        return View(league);
    }

    // GET /league/{slug}/settings
    [LeagueMember(CommissionerOnly = true)]
    public IActionResult Settings()
    {
        var league = HttpContext.Items[LeagueContextMiddleware.LeagueKey];
        return View(league);
    }
}
