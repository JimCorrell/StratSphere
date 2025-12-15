using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Infrastructure.Data;
using StratSphere.Shared.DTOs;

namespace StratSphere.Api.Controllers;

[ApiController]
[Route("api/leagues/{leagueId:guid}/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly StratSphereDbContext _context;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(StratSphereDbContext context, ILogger<TeamsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Add a player to a team's roster.
    /// </summary>
    [HttpPost("{teamId:guid}/roster")]
    public async Task<ActionResult<RosterEntryResponse>> AddToRoster(
        Guid leagueId,
        Guid teamId,
        [FromBody] AddToRosterRequest request)
    {
        var team = await _context.Teams.FirstOrDefaultAsync(t => t.LeagueId == leagueId && t.Id == teamId);
        if (team == null)
        {
            return NotFound("Team not found");
        }

        var player = await _context.Players.FindAsync(request.PlayerId);
        if (player == null)
        {
            return NotFound("Player not found");
        }

        // Check if player is already on a roster in this league
        var existingRoster = await _context.RosterEntries
            .AnyAsync(r => r.LeagueId == leagueId && r.PlayerId == request.PlayerId);
        if (existingRoster)
        {
            return Conflict("Player is already on a roster in this league");
        }

        var rosterEntry = new RosterEntry
        {
            LeagueId = leagueId,
            TeamId = teamId,
            PlayerId = request.PlayerId,
            AcquiredDate = DateTime.UtcNow,
            AcquiredVia = request.AcquiredVia,
            IsActive = request.IsActive,
            RosterPosition = request.RosterPosition,
            ContractYears = request.ContractYears,
            ContractSalary = request.ContractSalary,
            ContractYearRemaining = request.ContractYears
        };

        _context.RosterEntries.Add(rosterEntry);
        await _context.SaveChangesAsync();

        var response = new RosterEntryResponse(
            rosterEntry.Id,
            rosterEntry.TeamId,
            rosterEntry.PlayerId,
            player.FullName,
            player.PrimaryPosition,
            player.Level,
            rosterEntry.AcquiredDate,
            rosterEntry.AcquiredVia,
            rosterEntry.IsActive,
            rosterEntry.RosterPosition,
            rosterEntry.ContractYears,
            rosterEntry.ContractSalary,
            rosterEntry.ContractYearRemaining
        );

        return Created($"/api/leagues/{leagueId}/teams/{teamId}/roster/{rosterEntry.Id}", response);
    }

    /// <summary>
    /// Create a new team in a league.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TeamResponse>> CreateTeam(
        Guid leagueId,
        [FromBody] CreateTeamRequest request)
    {
        var ownerId = GetUserId();

        var league = await _context.Leagues.FindAsync(leagueId);
        if (league == null)
        {
            return NotFound("League not found");
        }

        // Check team count limit
        var teamCount = await _context.Teams.CountAsync(t => t.LeagueId == leagueId);
        if (teamCount >= league.MaxTeams)
        {
            return BadRequest($"League has reached maximum team count of {league.MaxTeams}");
        }

        // Check for duplicate name or abbreviation
        var duplicateName = await _context.Teams
            .AnyAsync(t => t.LeagueId == leagueId && t.Name == request.Name);
        if (duplicateName)
        {
            return Conflict("A team with this name already exists in the league");
        }

        var duplicateAbbr = await _context.Teams
            .AnyAsync(t => t.LeagueId == leagueId && t.Abbreviation == request.Abbreviation);
        if (duplicateAbbr)
        {
            return Conflict("A team with this abbreviation already exists in the league");
        }

        var owner = await _context.Users.FindAsync(ownerId);
        if (owner == null)
        {
            return NotFound("Owner user not found");
        }

        Subleague? subleague = null;
        Division? division = null;

        if (request.DivisionId.HasValue)
        {
            division = await _context.Divisions
                .Include(d => d.Subleague)
                .FirstOrDefaultAsync(d => d.Id == request.DivisionId.Value && d.LeagueId == leagueId);
            if (division == null)
            {
                return BadRequest("Division not found in this league");
            }
            subleague = division.Subleague;
        }
        else if (request.SubleagueId.HasValue)
        {
            subleague = await _context.Subleagues.FirstOrDefaultAsync(sl => sl.Id == request.SubleagueId.Value && sl.LeagueId == leagueId);
            if (subleague == null)
            {
                return BadRequest("Subleague not found in this league");
            }
        }

        var team = new Team
        {
            LeagueId = leagueId,
            Name = request.Name,
            Abbreviation = request.Abbreviation.ToUpper(),
            City = request.City,
            Conference = request.Conference,
            SubleagueId = subleague?.Id,
            DivisionId = division?.Id,
            DivisionName = request.DivisionName ?? division?.Name,
            OwnerId = ownerId
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created team {TeamId} '{TeamName}' in league {LeagueId}",
            team.Id, team.Name, leagueId);

        var response = new TeamResponse(
            team.Id,
            team.Name,
            team.Abbreviation,
            team.City,
            team.LogoUrl,
            team.SubleagueId,
            subleague?.Name,
            team.DivisionId,
            team.DivisionName,
            team.Conference,
            team.OwnerId,
            owner.DisplayName,
            0
        );

        return CreatedAtAction(nameof(GetTeam), new { leagueId, teamId = team.Id }, response);
    }

    /// <summary>
    /// Delete a team.
    /// </summary>
    [HttpDelete("{teamId:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid leagueId, Guid teamId)
    {
        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.LeagueId == leagueId && t.Id == teamId);

        if (team == null)
        {
            return NotFound();
        }

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted team {TeamId} from league {LeagueId}", teamId, leagueId);

        return NoContent();
    }

    /// <summary>
    /// Get a team's roster.
    /// </summary>
    [HttpGet("{teamId:guid}/roster")]
    public async Task<ActionResult<List<RosterEntryResponse>>> GetRoster(Guid leagueId, Guid teamId)
    {
        var roster = await _context.RosterEntries
            .Include(r => r.Player)
            .Where(r => r.LeagueId == leagueId && r.TeamId == teamId)
            .Select(r => new RosterEntryResponse(
                r.Id,
                r.TeamId,
                r.PlayerId,
                r.Player.FullName,
                r.Player.PrimaryPosition,
                r.Player.Level,
                r.AcquiredDate,
                r.AcquiredVia,
                r.IsActive,
                r.RosterPosition,
                r.ContractYears,
                r.ContractSalary,
                r.ContractYearRemaining
            ))
            .OrderBy(r => r.RosterPosition)
            .ThenBy(r => r.PlayerName)
            .ToListAsync();

        return Ok(roster);
    }

    /// <summary>
    /// Get a specific team.
    /// </summary>
    [HttpGet("{teamId:guid}")]
    public async Task<ActionResult<TeamResponse>> GetTeam(Guid leagueId, Guid teamId)
    {
        var team = await _context.Teams
            .Include(t => t.Owner)
            .Include(t => t.Subleague)
            .Include(t => t.Division)
            .Include(t => t.Roster)
            .Where(t => t.LeagueId == leagueId && t.Id == teamId)
            .Select(t => new TeamResponse(
                t.Id,
                t.Name,
                t.Abbreviation,
                t.City,
                t.LogoUrl,
                t.SubleagueId,
                t.Subleague != null ? t.Subleague.Name : null,
                t.DivisionId,
                t.DivisionName ?? (t.Division != null ? t.Division.Name : null),
                t.Conference,
                t.OwnerId,
                t.Owner.DisplayName,
                t.Roster.Count
            ))
            .FirstOrDefaultAsync();

        if (team == null)
        {
            return NotFound();
        }

        return Ok(team);
    }

    /// <summary>
    /// Get all teams in a league.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TeamResponse>>> GetTeams(Guid leagueId)
    {
        var teams = await _context.Teams
            .Include(t => t.Owner)
            .Include(t => t.Subleague)
            .Include(t => t.Division)
            .Include(t => t.Roster)
            .Where(t => t.LeagueId == leagueId)
            .Select(t => new TeamResponse(
                t.Id,
                t.Name,
                t.Abbreviation,
                t.City,
                t.LogoUrl,
                t.SubleagueId,
                t.Subleague != null ? t.Subleague.Name : null,
                t.DivisionId,
                t.DivisionName ?? (t.Division != null ? t.Division.Name : null),
                t.Conference,
                t.OwnerId,
                t.Owner.DisplayName,
                t.Roster.Count
            ))
            .ToListAsync();

        return Ok(teams);
    }

    /// <summary>
    /// Update a team.
    /// </summary>
    [HttpPut("{teamId:guid}")]
    public async Task<ActionResult<TeamResponse>> UpdateTeam(
        Guid leagueId,
        Guid teamId,
        [FromBody] UpdateTeamRequest request)
    {
        var team = await _context.Teams
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.LeagueId == leagueId && t.Id == teamId);

        if (team == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            var duplicateName = await _context.Teams
                .AnyAsync(t => t.LeagueId == leagueId && t.Name == request.Name && t.Id != teamId);
            if (duplicateName)
            {
                return Conflict("A team with this name already exists in the league");
            }
            team.Name = request.Name;
        }

        if (request.Abbreviation != null)
        {
            var duplicateAbbr = await _context.Teams
                .AnyAsync(t => t.LeagueId == leagueId && t.Abbreviation == request.Abbreviation && t.Id != teamId);
            if (duplicateAbbr)
            {
                return Conflict("A team with this abbreviation already exists in the league");
            }
            team.Abbreviation = request.Abbreviation.ToUpper();
        }

        if (request.City != null) team.City = request.City;
        if (request.Conference != null) team.Conference = request.Conference;
        if (request.DivisionName != null) team.DivisionName = request.DivisionName;

        if (request.SubleagueId.HasValue)
        {
            var subleague = await _context.Subleagues.FirstOrDefaultAsync(sl => sl.Id == request.SubleagueId.Value && sl.LeagueId == leagueId);
            if (subleague == null)
            {
                return BadRequest("Subleague not found in this league");
            }
            team.SubleagueId = subleague.Id;
        }

        if (request.DivisionId.HasValue)
        {
            var division = await _context.Divisions
                .Include(d => d.Subleague)
                .FirstOrDefaultAsync(d => d.Id == request.DivisionId.Value && d.LeagueId == leagueId);
            if (division == null)
            {
                return BadRequest("Division not found in this league");
            }

            team.DivisionId = division.Id;
            team.SubleagueId = division.SubleagueId;
            team.DivisionName = division.Name;
        }

        if (request.LogoUrl != null) team.LogoUrl = request.LogoUrl;

        await _context.SaveChangesAsync();

        return await GetTeam(leagueId, teamId);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }
}
