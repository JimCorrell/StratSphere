using System.ComponentModel.DataAnnotations;

namespace StratSphere.Core.Entities;

/// <summary>
/// Represents a player in a Strat-O-Matic league.
/// Links to MLB historical data via MlbPlayerId for reference.
/// </summary>
public class StratPlayer
{
    public int? AtBats { get; set; }

    /// <summary>
    /// Base salary for the player
    /// </summary>
    public decimal? BaseSalary { get; set; }

    /// <summary>
    /// Bats: L, R, or S (Switch)
    /// </summary>
    [MaxLength(1)]
    public string? Bats { get; set; }

    /// <summary>
    /// Current contract cost/cap hit
    /// </summary>
    public decimal? ContractCost { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Current season statistics (FanGraphs WAR, etc.)
    /// Stored as JSON or individual fields
    /// </summary>
    public decimal? CurrentSeasonWar { get; set; }

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Team decision: KEEP, RELEASE, etc.
    /// </summary>
    [MaxLength(20)]
    public string? Decision { get; set; }

    /// <summary>
    /// Display name (may differ from MLB name for nicknames, etc.)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;
    public int? GamesStarted { get; set; }
    public Guid Id { get; set; }
    public decimal? InningsPitched { get; set; }

    /// <summary>
    /// Whether this is an active player in the league
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether player is on 40-man roster
    /// </summary>
    public bool IsOn40Man { get; set; }
    public League League { get; set; } = null!;

    /// <summary>
    /// The league this player belongs to (multi-tenant isolation)
    /// </summary>
    [Required]
    public Guid LeagueId { get; set; }
    public MlbPeople? MlbPlayer { get; set; }

    /// <summary>
    /// Link to MLB historical player data (nullable for custom/fictional players)
    /// </summary>
    public string? MlbPlayerId { get; set; }

    /// <summary>
    /// Custom notes for league management
    /// </summary>
    public string? Notes { get; set; }
    public decimal? Points { get; set; }
    public decimal? PreviousSeasonWar { get; set; }

    /// <summary>
    /// Primary position code (e.g., "SS", "C", "OF", "P")
    /// Can be numeric (0-9) or letter codes
    /// </summary>
    [MaxLength(10)]
    public string? PrimaryPosition { get; set; }

    /// <summary>
    /// Qualifying offer status
    /// </summary>
    [MaxLength(50)]
    public string? QualifyingOffer { get; set; }

    /// <summary>
    /// Draft/signing information (e.g., "D2022118SPI04")
    /// </summary>
    [MaxLength(50)]
    public string? SignedInfo { get; set; }

    /// <summary>
    /// Player status in the league (e.g., "O2", "UFA", "MR5", "A4")
    /// </summary>
    [MaxLength(20)]
    public string? Status { get; set; }
    public StratTeam? StratTeam { get; set; }

    /// <summary>
    /// The Strat team this player is currently on
    /// </summary>
    public Guid? StratTeamId { get; set; }

    /// <summary>
    /// Throws: L or R
    /// </summary>
    [MaxLength(1)]
    public string? Throws { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
