namespace StratSphere.Web.Models.ViewModels.Player;

public class PlayerDetailViewModel
{
    public string    LahmanPlayerId    { get; set; } = "";
    public string    FullName          { get; set; } = "";
    public string    CardPosition      { get; set; } = "";
    public string?   Bats              { get; set; }
    public string?   Throws            { get; set; }
    public int       CardYear          { get; set; }
    public int?      BirthYear         { get; set; }
    public DateOnly? Debut             { get; set; }
    public DateOnly? FinalGame         { get; set; }

    public string LeagueName          { get; set; } = "";
    public string LeagueAbbreviation  { get; set; } = "";

    public IReadOnlyList<BattingSeasonRow>  CareerBatting  { get; set; } = [];
    public IReadOnlyList<PitchingSeasonRow> CareerPitching { get; set; } = [];

    public bool IsPitcher  => CardPosition is "SP" or "RP";
    public bool HasBatting  => CareerBatting.Any();
    public bool HasPitching => CareerPitching.Any();

    public string CareerSpan =>
        HasBatting  ? $"{CareerBatting.Min(r => r.YearId)}–{CareerBatting.Max(r => r.YearId)}"  :
        HasPitching ? $"{CareerPitching.Min(r => r.YearId)}–{CareerPitching.Max(r => r.YearId)}" :
        "";

    public class BattingSeasonRow
    {
        public int    YearId  { get; set; }
        public string TeamId  { get; set; } = "";
        public string? LgId   { get; set; }
        public int    G       { get; set; }
        public int    AB      { get; set; }
        public int    R       { get; set; }
        public int    H       { get; set; }
        public int    Doubles { get; set; }
        public int    Triples { get; set; }
        public int    HR      { get; set; }
        public int    RBI     { get; set; }
        public int    BB      { get; set; }
        public int    SO      { get; set; }
        public int    SB      { get; set; }
        public int    HBP     { get; set; }
        public int    SF      { get; set; }

        public decimal? BA  => AB > 0 ? Math.Round((decimal)H / AB, 3) : null;
        public decimal? OBP => (AB + BB + HBP + SF) > 0
            ? Math.Round((decimal)(H + BB + HBP) / (AB + BB + HBP + SF), 3) : null;
        public decimal? SLG => AB > 0
            ? Math.Round((decimal)(H + Doubles + Triples * 2 + HR * 3) / AB, 3) : null;
        public decimal? OPS => OBP.HasValue && SLG.HasValue ? Math.Round(OBP.Value + SLG.Value, 3) : null;

        public string BADisplay  => BA.HasValue  ? BA.Value.ToString(".000")  : "—";
        public string OBPDisplay => OBP.HasValue ? OBP.Value.ToString(".000") : "—";
        public string SLGDisplay => SLG.HasValue ? SLG.Value.ToString(".000") : "—";
        public string OPSDisplay => OPS.HasValue ? OPS.Value.ToString(".000") : "—";
    }

    public class PitchingSeasonRow
    {
        public int    YearId { get; set; }
        public string TeamId { get; set; } = "";
        public string? LgId  { get; set; }
        public int    W      { get; set; }
        public int    L      { get; set; }
        public int    G      { get; set; }
        public int    GS     { get; set; }
        public int    SV     { get; set; }
        public int    IPOuts { get; set; }
        public int    H      { get; set; }
        public int    ER     { get; set; }
        public int    BB     { get; set; }
        public int    SO     { get; set; }

        public decimal? IP   => IPOuts > 0 ? Math.Round(IPOuts / 3m, 1) : null;
        public decimal? ERA  => IPOuts > 0 ? Math.Round((decimal)ER / IPOuts * 27, 2) : null;
        public decimal? WHIP => IPOuts > 0 ? Math.Round((decimal)(BB + H) / IPOuts * 3, 3) : null;
        public decimal? K9   => IPOuts > 0 ? Math.Round((decimal)SO / IPOuts * 27, 2) : null;

        public string IPDisplay   => IP.HasValue   ? IP.Value.ToString("0.0")   : "—";
        public string ERADisplay  => ERA.HasValue  ? ERA.Value.ToString("0.00") : "—";
        public string WHIPDisplay => WHIP.HasValue ? WHIP.Value.ToString("0.000") : "—";
        public string K9Display   => K9.HasValue   ? K9.Value.ToString("0.0")  : "—";
    }
}
