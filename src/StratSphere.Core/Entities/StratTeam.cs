using System.ComponentModel.DataAnnotations;

namespace StratSphere.Core.Entities;

/// <summary>
/// Represents a team in a Strat-O-Matic league.
/// Separate from the main Team entity which handles fantasy league teams.
/// </summary>
public class StratTeam
{

    /// <summary>
    /// Team abbreviation (e.g., "NEE", "CHH", "BBS")
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Abbreviation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Division or conference
    /// </summary>
    [MaxLength(50)]
    public string? Division { get; set; }
    public Guid Id { get; set; }

    /// <summary>
    /// Whether this team is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    public League League { get; set; } = null!;

    /// <summary>
    /// The league this team belongs to (multi-tenant isolation)
    /// </summary>
    [Required]
    public Guid LeagueId { get; set; }

    /// <summary>
    /// Team logo URL
    /// </summary>
    public string? LogoUrl { get; set; }
    public int Losses { get; set; }

    /// <summary>
    /// Full team name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Team owner/manager name
    /// </summary>
    [MaxLength(100)]
    public string? Owner { get; set; }

    /// <summary>
    /// Players on this team
    /// </summary>
    public ICollection<StratPlayer> Players { get; set; } = new List<StratPlayer>();

    /// <summary>
    /// Primary team color (hex)
    /// </summary>
    [MaxLength(7)]
    public string? PrimaryColor { get; set; }

    /// <summary>
    /// Number of players on 40-man roster
    /// </summary>
    public int RosterCount40Man { get; set; }

    /// <summary>
    /// Secondary team color (hex)
    /// </summary>
    [MaxLength(7)]
    public string? SecondaryColor { get; set; }

    /// <summary>
    /// Total roster count
    /// </summary>
    public int TotalRosterCount { get; set; }

    /// <summary>
    /// Total salary cap used
    /// </summary>
    public decimal TotalSalary { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Season record (Wins-Losses)
    /// </summary>
    public int Wins { get; set; }
}
