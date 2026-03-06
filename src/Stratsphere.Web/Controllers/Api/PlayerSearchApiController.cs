using Microsoft.AspNetCore.Mvc;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Web.Controllers.Api;

[ApiController]
[Route("api/players")]
public class PlayerSearchApiController(ILahmanRepository lahmanRepo) : ControllerBase
{
    /// <summary>
    /// Typeahead endpoint for searching player cards.
    /// GET /api/players/search?q=schmidt&year=1986&pitchers=false
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int? year,
        [FromQuery] bool pitchers = false)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(Array.Empty<object>());

        var results = await lahmanRepo.SearchCardsAsync(q, year, pitchers, limit: 20);

        return Ok(results.Select(r => new
        {
            playerId   = r.Person.PlayerId,
            name       = r.Person.FullName,
            position   = r.PrimaryPosition,
            cardYear   = year,
            debut      = r.Person.Debut?.Year,
            finalGame  = r.Person.FinalGame?.Year,
            bats       = r.Person.Bats,
            throws     = r.Person.Throws
        }));
    }
}
