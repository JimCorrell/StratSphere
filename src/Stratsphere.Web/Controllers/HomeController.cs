using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Interfaces;
using System.Security.Claims;

namespace Stratsphere.Web.Controllers;

public class HomeController(ILeagueRepository leagueRepo) : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Dashboard));
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Dashboard()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var leagues = await leagueRepo.GetByUserIdAsync(userId);
        return View(leagues);
    }
}
