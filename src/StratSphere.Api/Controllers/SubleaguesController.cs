using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Infrastructure.Data;
using StratSphere.Shared.DTOs;

namespace StratSphere.Api.Controllers;

[ApiController]
[Route("api/leagues/{leagueId:guid}/subleagues")]
[Authorize]
public class SubleaguesController : ControllerBase
{
    private readonly StratSphereDbContext _context;
    private readonly ILogger<SubleaguesController> _logger;

    public SubleaguesController(StratSphereDbContext context, ILogger<SubleaguesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Create a new subleague.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SubleagueResponse>> CreateSubleague(
        Guid leagueId,
        [FromBody] CreateSubleagueRequest request)
    {
        var league = await _context.Leagues.FindAsync(leagueId);
        if (league == null)
        {
            return NotFound("League not found");
        }

        // Check for duplicate name
        var duplicateName = await _context.Subleagues
            .AnyAsync(s => s.LeagueId == leagueId && s.Name.ToLower() == request.Name.ToLower());
        if (duplicateName)
        {
            return Conflict("A subleague with this name already exists");
        }

        var subleague = new Subleague
        {
            LeagueId = leagueId,
            Name = request.Name,
            Abbreviation = request.Abbreviation?.ToUpper()
        };

        _context.Subleagues.Add(subleague);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created subleague {SubleagueId} '{SubleagueName}' in league {LeagueId}",
            subleague.Id, subleague.Name, leagueId);

        return CreatedAtAction(nameof(GetSubleague), new { leagueId, subleagueId = subleague.Id },
            new SubleagueResponse(subleague.Id, subleague.Name, subleague.Abbreviation, 0));
    }

    /// <summary>
    /// Delete a subleague.
    /// </summary>
    [HttpDelete("{subleagueId:guid}")]
    public async Task<IActionResult> DeleteSubleague(Guid leagueId, Guid subleagueId)
    {
        var subleague = await _context.Subleagues
            .FirstOrDefaultAsync(s => s.Id == subleagueId && s.LeagueId == leagueId);

        if (subleague == null)
        {
            return NotFound();
        }

        _context.Subleagues.Remove(subleague);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted subleague {SubleagueId} from league {LeagueId}", subleagueId, leagueId);

        return NoContent();
    }

    /// <summary>
    /// Get a specific subleague.
    /// </summary>
    [HttpGet("{subleagueId:guid}")]
    public async Task<ActionResult<SubleagueResponse>> GetSubleague(Guid leagueId, Guid subleagueId)
    {
        var subleague = await _context.Subleagues
            .Include(s => s.Divisions)
            .Where(s => s.Id == subleagueId && s.LeagueId == leagueId)
            .FirstOrDefaultAsync();

        if (subleague == null)
        {
            return NotFound();
        }

        return Ok(new SubleagueResponse(subleague.Id, subleague.Name, subleague.Abbreviation, subleague.Divisions.Count));
    }

    /// <summary>
    /// Get all subleagues in a league.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SubleagueResponse>>> GetSubleagues(Guid leagueId)
    {
        var league = await _context.Leagues.FindAsync(leagueId);
        if (league == null)
        {
            return NotFound("League not found");
        }

        var subleagues = await _context.Subleagues
            .Include(s => s.Divisions)
            .Where(s => s.LeagueId == leagueId)
            .Select(s => new SubleagueResponse(s.Id, s.Name, s.Abbreviation, s.Divisions.Count))
            .OrderBy(s => s.Name)
            .ToListAsync();

        return Ok(subleagues);
    }

    /// <summary>
    /// Update a subleague.
    /// </summary>
    [HttpPut("{subleagueId:guid}")]
    public async Task<ActionResult<SubleagueResponse>> UpdateSubleague(
        Guid leagueId,
        Guid subleagueId,
        [FromBody] UpdateSubleagueRequest request)
    {
        var subleague = await _context.Subleagues
            .Include(s => s.Divisions)
            .FirstOrDefaultAsync(s => s.Id == subleagueId && s.LeagueId == leagueId);

        if (subleague == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            var duplicateName = await _context.Subleagues
                .AnyAsync(s => s.LeagueId == leagueId && s.Name.ToLower() == request.Name.ToLower() && s.Id != subleagueId);
            if (duplicateName)
            {
                return Conflict("A subleague with this name already exists");
            }
            subleague.Name = request.Name;
        }

        if (request.Abbreviation != null)
        {
            subleague.Abbreviation = request.Abbreviation.ToUpper();
        }

        await _context.SaveChangesAsync();

        return Ok(new SubleagueResponse(subleague.Id, subleague.Name, subleague.Abbreviation, subleague.Divisions.Count));
    }
}
