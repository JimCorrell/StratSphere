namespace Stratsphere.Core.Entities.Lahman;

/// <summary>Read-only. Maps to lahman.people. Never modified by the app.</summary>
public class LahmanPerson
{
    public string PlayerId { get; set; } = string.Empty;  // e.g. "ruthba01"
    public int? BirthYear { get; set; }
    public int? BirthMonth { get; set; }
    public int? BirthDay { get; set; }
    public string? NameFirst { get; set; }
    public string? NameLast { get; set; }
    public string? NameGiven { get; set; }
    public string? Bats { get; set; }
    public string? Throws { get; set; }
    public DateOnly? Debut { get; set; }
    public DateOnly? FinalGame { get; set; }
    public string? BbrefId { get; set; }
    public string? RetroId { get; set; }

    public string FullName => $"{NameFirst} {NameLast}".Trim();
}
