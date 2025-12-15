namespace StratSphere.Core.Entities;

/// <summary>
/// MLB People table - biographical information for all players and managers
/// </summary>
public class MlbPeople
{
    public char? Bats { get; set; }
    public string? BbRefUrl { get; set; }
    public string? BirthCity { get; set; }
    public string? BirthCountry { get; set; }
    public int? BirthDay { get; set; }
    public int? BirthMonth { get; set; }
    public string? BirthState { get; set; }
    public int? BirthYear { get; set; }
    public string? DeathCity { get; set; }
    public string? DeathCountry { get; set; }
    public int? DeathDay { get; set; }
    public int? DeathMonth { get; set; }
    public string? DeathState { get; set; }
    public int? DeathYear { get; set; }
    public DateTime? Debut { get; set; }
    public DateTime? FinalGame { get; set; }
    public int? Height { get; set; }
    public int Id { get; set; }
    public string? NameFirst { get; set; }
    public string? NameGiven { get; set; }
    public string? NameLast { get; set; }
    public string? PlayerId { get; set; } // playerID from MLB
    public string? RetroSheetId { get; set; }
    public char? Throws { get; set; }
    public int? Weight { get; set; }
}
