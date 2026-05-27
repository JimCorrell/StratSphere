namespace StratSphere.Core.Entities;

public class SomImportLog
{
    public Guid     Id               { get; set; }
    public Guid     SeasonId         { get; set; }
    public DateTime ExportDate       { get; set; }
    public DateTime ImportedAt       { get; set; }
    public int      GamesImported    { get; set; }
    public int      BattersImported  { get; set; }
    public int      PitchersImported { get; set; }
    public int      UnmatchedPlayers { get; set; }

    public Season Season { get; set; } = null!;
}
