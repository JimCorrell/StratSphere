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

    // GET /Home/Error?statusCode={code}  — used by UseStatusCodePagesWithReExecute and UseExceptionHandler
    [AllowAnonymous]
    public IActionResult Error(int statusCode = 0)
    {
        // Preserve the original status code in the response (re-execute resets it to 200)
        if (statusCode > 0)
            Response.StatusCode = statusCode;

        return View(statusCode);
    }
}
