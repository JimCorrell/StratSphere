using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Core.Interfaces;
using StratSphere.Web.Models.ViewModels.League;
using System.Security.Claims;

namespace StratSphere.Web.Controllers;

public class HomeController(ILeagueRepository leagueRepo) : Controller
{
    // GET /
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Dashboard));

        return View();
    }

    // GET /home/dashboard
    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var leagues = await leagueRepo.GetByUserIdAsync(userId);

        var model = new LeagueIndexViewModel
        {
            Leagues = leagues.Select(l => new LeagueIndexViewModel.LeagueSummary
            {
                Name = l.Name,
                Slug = l.Slug,
                Status = l.Status,
                MemberCount = l.Members.Count,
                Role = l.Members.FirstOrDefault(m => m.UserId == userId)?.Role.ToString() ?? ""
            })
        };

        return View(model);
    }
}
