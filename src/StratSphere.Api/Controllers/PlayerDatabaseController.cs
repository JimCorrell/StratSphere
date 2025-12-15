using Microsoft.AspNetCore.Mvc;
using StratSphere.Infrastructure.Repositories;

namespace StratSphere.Api.Controllers;

/// <summary>
/// Controller for accessing MLB historical player database.
/// Provides access to the complete Lahman database of historical player and team statistics.
/// Separate from the main Players endpoint which handles league-specific player data.
/// </summary>
[ApiController]
[Route("api/player-database")]
public class PlayerDatabaseController : ControllerBase
{
    private readonly ILogger<PlayerDatabaseController> _logger;
    private readonly IPlayerRepository _playerRepository;

    public PlayerDatabaseController(IPlayerRepository playerRepository, ILogger<PlayerDatabaseController> logger)
    {
        _playerRepository = playerRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get players by position from the MLB database
    /// </summary>
    [HttpGet("by-position")]
    public async Task<IActionResult> GetByPosition([FromQuery] string position, [FromQuery] int limit = 50)
    {
        if (string.IsNullOrWhiteSpace(position))
            return BadRequest("Position is required");

        var players = await _playerRepository.GetPlayersByPositionAsync(position, Math.Min(limit, 200));
        return Ok(players);
    }

    /// <summary>
    /// Get all Hall of Fame players from the MLB database
    /// </summary>
    [HttpGet("hall-of-fame")]
    public async Task<IActionResult> GetHallOfFame()
    {
        var players = await _playerRepository.GetHallOfFameAsync();
        return Ok(players);
    }

    /// <summary>
    /// Get a specific player from the MLB database
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayer(int id)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(id);
        if (player == null)
            return NotFound($"Player with ID {id} not found");

        return Ok(player);
    }

    /// <summary>
    /// Get player batting stats by player ID
    /// </summary>
    [HttpGet("{playerId}/batting-stats")]
    public async Task<IActionResult> GetPlayerBattingStats(string playerId, [FromQuery] int? startYear, [FromQuery] int? endYear)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(int.Parse(playerId));
        if (player == null)
            return NotFound($"Player with ID {playerId} not found");

        var stats = await _playerRepository.GetPlayerBattingStatsAsync(player.PlayerId ?? playerId, startYear, endYear);
        return Ok(new
        {
            player = player,
            battingStats = stats
        });
    }

    /// <summary>
    /// Get player fielding stats by player ID
    /// </summary>
    [HttpGet("{playerId}/fielding-stats")]
    public async Task<IActionResult> GetPlayerFieldingStats(string playerId, [FromQuery] int? startYear, [FromQuery] int? endYear)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(int.Parse(playerId));
        if (player == null)
            return NotFound($"Player with ID {playerId} not found");

        var stats = await _playerRepository.GetPlayerFieldingStatsAsync(player.PlayerId ?? playerId, startYear, endYear);
        return Ok(new
        {
            player = player,
            fieldingStats = stats
        });
    }

    /// <summary>
    /// Get player pitching stats by player ID
    /// </summary>
    [HttpGet("{playerId}/pitching-stats")]
    public async Task<IActionResult> GetPlayerPitchingStats(string playerId, [FromQuery] int? startYear, [FromQuery] int? endYear)
    {
        var player = await _playerRepository.GetPlayerByIdAsync(int.Parse(playerId));
        if (player == null)
            return NotFound($"Player with ID {playerId} not found");

        var stats = await _playerRepository.GetPlayerPitchingStatsAsync(player.PlayerId ?? playerId, startYear, endYear);
        return Ok(new
        {
            player = player,
            pitchingStats = stats
        });
    }

    /// <summary>
    /// Search for players by name in the MLB database
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchPlayers([FromQuery] string? searchTerm, [FromQuery] int limit = 50)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        var players = await _playerRepository.SearchPlayersAsync(searchTerm, Math.Min(limit, 200));
        return Ok(players);
    }
}
