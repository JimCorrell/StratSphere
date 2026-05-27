using System.IO.Compression;
using System.Text;
using Microsoft.Data.Sqlite;

namespace StratSphere.Web.Services;

/// <summary>
/// Parses a Strat-O-Matic PC Baseball .lzp export archive (standard ZIP renamed).
/// Extracts game results, player stats, and the SOM team order from the binary game files.
/// </summary>
public class SomImportService
{
    public SomParseResult Parse(Stream lzpStream)
    {
        using var archive = new ZipArchive(lzpStream, ZipArchiveMode.Read, leaveOpen: true);

        var teamOrder  = ExtractTeamOrder(archive);
        var exportDate = ExtractExportDate(archive);
        var dbPath     = ExtractDb3(archive);

        try
        {
            var games    = QueryGames(dbPath, teamOrder.Length);
            var batting  = QueryBatting(dbPath);
            var pitching = QueryPitching(dbPath);
            return new SomParseResult(teamOrder, games, batting, pitching, exportDate);
        }
        finally
        {
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }

    // ── Export date ───────────────────────────────────────────────────────────

    private static DateTime ExtractExportDate(ZipArchive archive)
    {
        var timestamps = archive.Entries
            .Where(e => e.LastWriteTime != DateTimeOffset.MinValue)
            .Select(e => e.LastWriteTime.UtcDateTime)
            .ToList();
        return timestamps.Count > 0 ? timestamps.Max() : DateTime.UtcNow;
    }

    // ── Team order ────────────────────────────────────────────────────────────

    private static string[] ExtractTeamOrder(ZipArchive archive)
    {
        // Any .H01 game file works — team order is in header bytes 25..144
        // Format: 30 × (3-char name + \x00) starting at byte 25
        var entry = archive.Entries
            .FirstOrDefault(e => e.FullName.EndsWith(".H01", StringComparison.OrdinalIgnoreCase)
                               && e.FullName.StartsWith("Export/", StringComparison.OrdinalIgnoreCase));

        if (entry is null)
            return [];

        using var stream = entry.Open();
        var header = new byte[145];
        var read = stream.Read(header, 0, header.Length);

        if (read < 145) return [];

        return Enumerable.Range(0, 30)
            .Select(i => Encoding.ASCII.GetString(header, 25 + i * 4, 3).TrimEnd('\0'))
            .ToArray();
    }

    // ── DB3 extraction ────────────────────────────────────────────────────────

    private static string ExtractDb3(ZipArchive archive)
    {
        var entry = archive.Entries
            .FirstOrDefault(e => e.FullName.EndsWith(".DB3", StringComparison.OrdinalIgnoreCase)
                               && e.FullName.StartsWith("Stats/", StringComparison.OrdinalIgnoreCase)
                               && e.FullName.Contains("Logs", StringComparison.OrdinalIgnoreCase));

        if (entry is null)
            throw new InvalidOperationException("Could not find Stats/*Logs.DB3 in the archive.");

        var tempPath = Path.Combine(Path.GetTempPath(), $"som_{Guid.NewGuid()}.db3");
        using var dst = File.OpenWrite(tempPath);
        using var src = entry.Open();
        src.CopyTo(dst);
        return tempPath;
    }

    // ── Game results ──────────────────────────────────────────────────────────

    private static List<SomGame> QueryGames(string dbPath, int teamCount)
    {
        var results = new List<SomGame>();

        const string sql = """
            SELECT a.Day, COALESCE(a.Which, 0) AS GameNumber,
                   a.Team AS HomeTeam, a.OtherTeam AS AwayTeam,
                   a.HomeRuns, b.AwayRuns
            FROM (
                SELECT Day, COALESCE(Which, 0) AS Which, Team, OtherTeam, SUM(Runs) AS HomeRuns
                FROM BatterLog
                GROUP BY Day, COALESCE(Which, 0), Team, OtherTeam
            ) a
            JOIN (
                SELECT Day, COALESCE(Which, 0) AS Which, Team, OtherTeam, SUM(Runs) AS AwayRuns
                FROM BatterLog
                GROUP BY Day, COALESCE(Which, 0), Team, OtherTeam
            ) b ON a.Day = b.Day AND a.Which = b.Which
                AND a.Team = b.OtherTeam AND a.OtherTeam = b.Team
            WHERE a.Team < a.OtherTeam
            ORDER BY a.Day, a.Which, a.Team
            """;

        using var conn = new SqliteConnection($"Data Source={dbPath};Mode=ReadOnly");
        conn.Open();
        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new SomGame(
                Day:        reader.GetInt32(0),
                GameNumber: reader.GetInt32(1),
                HomeTeam:   reader.GetInt32(2),
                AwayTeam:   reader.GetInt32(3),
                HomeRuns:   reader.GetInt32(4),
                AwayRuns:   reader.GetInt32(5)
            ));
        }

        return results;
    }

    // ── Batting stats ─────────────────────────────────────────────────────────

    private static List<SomBatterStats> QueryBatting(string dbPath)
    {
        var results = new List<SomBatterStats>();

        const string sql = """
            SELECT Key,
                   SUM(AtBats1 + AtBats2)  AS AB,
                   SUM(Hits1   + Hits2)    AS H,
                   SUM(BB)                 AS BB,
                   SUM(Ks)                 AS SO,
                   SUM(HomeRuns)           AS HR,
                   SUM(Runs)               AS R,
                   SUM(RBI)                AS RBI,
                   SUM(Steals)             AS SB,
                   COUNT(*)                AS G,
                   (SELECT Team FROM BatterLog b2 WHERE b2.Key = b.Key
                    GROUP BY Team ORDER BY SUM(AtBats1+AtBats2) DESC LIMIT 1) AS PrimaryTeam
            FROM BatterLog b
            WHERE Key LIKE 'B%'
            GROUP BY Key
            HAVING SUM(AtBats1 + AtBats2) > 0
            """;

        using var conn = new SqliteConnection($"Data Source={dbPath};Mode=ReadOnly");
        conn.Open();
        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var key = reader.GetString(0);
            if (!TryParseKey(key, out var parsed)) continue;

            results.Add(new SomBatterStats(
                Key:         key,
                CardYear:    parsed.Year,
                CardTeam:    parsed.Team,
                FirstInitial: parsed.FirstInitial,
                LastName:    parsed.LastName,
                AB:          reader.GetInt32(1),
                H:           reader.GetInt32(2),
                BB:          reader.GetInt32(3),
                SO:          reader.GetInt32(4),
                HR:          reader.GetInt32(5),
                R:           reader.GetInt32(6),
                RBI:         reader.GetInt32(7),
                SB:          reader.GetInt32(8),
                G:           reader.GetInt32(9),
                PrimaryTeamIndex: reader.IsDBNull(10) ? -1 : reader.GetInt32(10)
            ));
        }

        return results;
    }

    // ── Pitching stats ────────────────────────────────────────────────────────

    private static List<SomPitcherStats> QueryPitching(string dbPath)
    {
        var results = new List<SomPitcherStats>();

        const string sql = """
            SELECT Key,
                   SUM(Innings * 3 + Thirds) AS IPOuts,
                   SUM(ERuns)                AS ER,
                   SUM(Hits)                 AS H,
                   SUM(BB)                   AS BB,
                   SUM(Ks)                   AS SO,
                   SUM(HomeRuns)             AS HR,
                   SUM(Won)                  AS W,
                   SUM(Lost)                 AS L,
                   SUM(Saves)                AS SV,
                   COUNT(*)                  AS G,
                   SUM(CASE WHEN Innings >= 5 THEN 1 ELSE 0 END) AS GS,
                   (SELECT Team FROM PitcherLog p2 WHERE p2.Key = p.Key
                    GROUP BY Team ORDER BY SUM(Innings) DESC LIMIT 1) AS PrimaryTeam
            FROM PitcherLog p
            WHERE Key LIKE 'P%'
            GROUP BY Key
            HAVING SUM(Innings) > 0
            """;

        using var conn = new SqliteConnection($"Data Source={dbPath};Mode=ReadOnly");
        conn.Open();
        using var cmd = new SqliteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var key = reader.GetString(0);
            if (!TryParseKey(key, out var parsed)) continue;

            results.Add(new SomPitcherStats(
                Key:          key,
                CardYear:     parsed.Year,
                CardTeam:     parsed.Team,
                FirstInitial: parsed.FirstInitial,
                LastName:     parsed.LastName,
                IPOuts:       reader.GetInt32(1),
                ER:           reader.GetInt32(2),
                H:            reader.GetInt32(3),
                BB:           reader.GetInt32(4),
                SO:           reader.GetInt32(5),
                HR:           reader.GetInt32(6),
                W:            reader.GetInt32(7),
                L:            reader.GetInt32(8),
                SV:           reader.GetInt32(9),
                G:            reader.GetInt32(10),
                GS:           reader.GetInt32(11),
                PrimaryTeamIndex: reader.IsDBNull(12) ? -1 : reader.GetInt32(12)
            ));
        }

        return results;
    }

    // ── Key parsing ───────────────────────────────────────────────────────────

    // Key format: B2025SDNLArraez  or  P2025DEATSkubal
    //             [0] type, [1..4] year, [5..7] team, [8] initial, [9..] lastName
    private static bool TryParseKey(string key, out (int Year, string Team, char FirstInitial, string LastName) result)
    {
        result = default;
        if (key.Length < 10) return false;
        if (!int.TryParse(key[1..5], out var year)) return false;
        result = (year, key[5..8], key[8], key[9..]);
        return true;
    }
}

// ── Result types ──────────────────────────────────────────────────────────────

public sealed record SomParseResult(
    string[]          TeamOrder,   // 30 SOM team abbreviations in index order
    List<SomGame>     Games,
    List<SomBatterStats>  Batting,
    List<SomPitcherStats> Pitching,
    DateTime          ExportDate   // max LastWriteTime across ZIP entries
);

public sealed record SomGame(int Day, int GameNumber, int HomeTeam, int AwayTeam, int HomeRuns, int AwayRuns);

public sealed record SomBatterStats(
    string Key, int CardYear, string CardTeam,
    char FirstInitial, string LastName,
    int AB, int H, int BB, int SO, int HR, int R, int RBI, int SB, int G,
    int PrimaryTeamIndex
);

public sealed record SomPitcherStats(
    string Key, int CardYear, string CardTeam,
    char FirstInitial, string LastName,
    int IPOuts, int ER, int H, int BB, int SO, int HR,
    int W, int L, int SV, int G, int GS,
    int PrimaryTeamIndex
);
