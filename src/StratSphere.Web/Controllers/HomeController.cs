using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StratSphere.Web.Controllers;

public class HomeController : Controller
{
    // GET /
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "League");

        return View();
    }

    // GET /home/dashboard — preserved for bookmarks
    [Authorize]
    public IActionResult Dashboard() => RedirectToAction("Index", "League");
}
