using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using System.Globalization;

namespace StratSphere.DataImport.Importers;

public class BattingImporter : IImporter
{
    public string TableName => "lahman.batting";

    public async Task<int> ImportAsync(NpgsqlConnection conn, string sourceDir)
    {
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                DROP TABLE IF EXISTS lahman.batting;
                CREATE TABLE lahman.batting (
                    player_id VARCHAR(9), year_id INT, stint INT,
                    team_id VARCHAR(3), lg_id VARCHAR(3),
                    g INT, ab INT, r INT, h INT, doubles INT, triples INT,
                    hr INT, rbi INT, sb INT, cs INT, bb INT, so INT,
                    ibb INT, hbp INT, sh INT, sf INT, g_idp INT,
                    PRIMARY KEY (player_id, year_id, stint)
                );
                CREATE INDEX idx_batting_player_year ON lahman.batting (player_id, year_id);
                """;
            await cmd.ExecuteNonQueryAsync();
        }

        var csvPath = Path.Combine(sourceDir, "Batting.csv");
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            { HasHeaderRecord = true, MissingFieldFound = null, BadDataFound = null });

        var records = csv.GetRecords<dynamic>().ToList();

        await using var writer = conn.BeginBinaryImport(
            "COPY lahman.batting (player_id,year_id,stint,team_id,lg_id,g,ab,r,h,doubles,triples,hr,rbi,sb,cs,bb,so,ibb,hbp,sh,sf,g_idp) FROM STDIN (FORMAT BINARY)");

        foreach (var r in records)
        {
            var d = (IDictionary<string, object>)r;
            await writer.StartRowAsync();
            Write(writer, S(d,"playerID")); Write(writer, I(d,"yearID")); Write(writer, I(d,"stint"));
            Write(writer, S(d,"teamID")); Write(writer, S(d,"lgID"));
            Write(writer, I(d,"G")); Write(writer, I(d,"AB")); Write(writer, I(d,"R")); Write(writer, I(d,"H"));
            Write(writer, I(d,"2B")); Write(writer, I(d,"3B")); Write(writer, I(d,"HR")); Write(writer, I(d,"RBI"));
            Write(writer, I(d,"SB")); Write(writer, I(d,"CS")); Write(writer, I(d,"BB")); Write(writer, I(d,"SO"));
            Write(writer, I(d,"IBB")); Write(writer, I(d,"HBP")); Write(writer, I(d,"SH")); Write(writer, I(d,"SF"));
            Write(writer, I(d,"GIDP"));
        }

        await writer.CompleteAsync();
        return records.Count;
    }

    private static void Write(NpgsqlBinaryImporter w, object? v)
    {
        if (v is null) w.WriteNull();
        else if (v is string s) w.Write(s, NpgsqlTypes.NpgsqlDbType.Varchar);
        else if (v is int i) w.Write(i, NpgsqlTypes.NpgsqlDbType.Integer);
    }

    private static string? S(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && v?.ToString() is { Length: > 0 } s ? s : null;
    private static int? I(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && int.TryParse(v?.ToString(), out var i) ? i : (int?)null;
}
