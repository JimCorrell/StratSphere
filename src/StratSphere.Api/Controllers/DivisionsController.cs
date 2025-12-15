using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Infrastructure.Data;
using StratSphere.Shared.DTOs;

namespace StratSphere.Api.Controllers;

[ApiController]
[Route("api/leagues/{leagueId:guid}/subleagues/{subleagueId:guid}/divisions")]
[Authorize]
public class DivisionsController : ControllerBase
{
    private readonly StratSphereDbContext _context;
    private readonly ILogger<DivisionsController> _logger;

    public DivisionsController(StratSphereDbContext context, ILogger<DivisionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Create a new division within a subleague.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DivisionResponse>> CreateDivision(
        Guid leagueId,
        Guid subleagueId,
        [FromBody] CreateDivisionRequest request)
    {
        var subleague = await _context.Subleagues
            .FirstOrDefaultAsync(s => s.Id == subleagueId && s.LeagueId == leagueId);

        if (subleague == null)
        {
            return NotFound("Subleague not found");
        }

        // Check for duplicate name within subleague
        var duplicateName = await _context.Divisions
            .AnyAsync(d => d.SubleagueId == subleagueId && d.Name.ToLower() == request.Name.ToLower());
        if (duplicateName)
        {
            return Conflict("A division with this name already exists in this subleague");
        }

        var division = new Division
        {
            LeagueId = leagueId,
            SubleagueId = subleagueId,
            Name = request.Name,
            Abbreviation = request.Abbreviation?.ToUpper()
        };

        _context.Divisions.Add(division);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created division {DivisionId} '{DivisionName}' in subleague {SubleagueId}",
            division.Id, division.Name, subleagueId);

        return CreatedAtAction(nameof(GetDivision), new { leagueId, subleagueId, divisionId = division.Id },
            new DivisionResponse(division.Id, division.Name, division.Abbreviation, 0));
    }

    /// <summary>
    /// Delete a division.
    /// </summary>
    [HttpDelete("{divisionId:guid}")]
    public async Task<IActionResult> DeleteDivision(Guid leagueId, Guid subleagueId, Guid divisionId)
    {
        var division = await _context.Divisions
            .FirstOrDefaultAsync(d => d.Id == divisionId && d.SubleagueId == subleagueId && d.LeagueId == leagueId);

        if (division == null)
        {
            return NotFound();
        }

        _context.Divisions.Remove(division);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted division {DivisionId} from subleague {SubleagueId}", divisionId, subleagueId);

        return NoContent();
    }

    /// <summary>
    /// Get a specific division.
    /// </summary>
    [HttpGet("{divisionId:guid}")]
    public async Task<ActionResult<DivisionResponse>> GetDivision(Guid leagueId, Guid subleagueId, Guid divisionId)
    {
        var division = await _context.Divisions
            .Include(d => d.Teams)
            .Where(d => d.Id == divisionId && d.SubleagueId == subleagueId && d.LeagueId == leagueId)
            .FirstOrDefaultAsync();

        if (division == null)
        {
            return NotFound();
        }

        return Ok(new DivisionResponse(division.Id, division.Name, division.Abbreviation, division.Teams.Count));
    }

    /// <summary>
    /// Get all divisions in a subleague.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DivisionResponse>>> GetDivisions(Guid leagueId, Guid subleagueId)
    {
        var subleague = await _context.Subleagues
            .FirstOrDefaultAsync(s => s.Id == subleagueId && s.LeagueId == leagueId);

        if (subleague == null)
        {
            return NotFound("Subleague not found");
        }

        var divisions = await _context.Divisions
            .Include(d => d.Teams)
            .Where(d => d.SubleagueId == subleagueId && d.LeagueId == leagueId)
            .Select(d => new DivisionResponse(d.Id, d.Name, d.Abbreviation, d.Teams.Count))
            .OrderBy(d => d.Name)
            .ToListAsync();

        return Ok(divisions);
    }

    /// <summary>
    /// Update a division.
    /// </summary>
    [HttpPut("{divisionId:guid}")]
    public async Task<ActionResult<DivisionResponse>> UpdateDivision(
        Guid leagueId,
        Guid subleagueId,
        Guid divisionId,
        [FromBody] UpdateDivisionRequest request)
    {
        var division = await _context.Divisions
            .Include(d => d.Teams)
            .FirstOrDefaultAsync(d => d.Id == divisionId && d.SubleagueId == subleagueId && d.LeagueId == leagueId);

        if (division == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            var duplicateName = await _context.Divisions
                .AnyAsync(d => d.SubleagueId == subleagueId && d.Name.ToLower() == request.Name.ToLower() && d.Id != divisionId);
            if (duplicateName)
            {
                return Conflict("A division with this name already exists in this subleague");
            }
            division.Name = request.Name;
        }

        if (request.Abbreviation != null)
        {
            division.Abbreviation = request.Abbreviation.ToUpper();
        }

        await _context.SaveChangesAsync();

        return Ok(new DivisionResponse(division.Id, division.Name, division.Abbreviation, division.Teams.Count));
    }
}
