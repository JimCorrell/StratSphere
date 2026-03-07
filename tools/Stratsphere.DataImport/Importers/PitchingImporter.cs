using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using System.Globalization;

namespace StratSphere.DataImport.Importers;

public class PitchingImporter : IImporter
{
    public string TableName => "lahman.pitching";

    public async Task<int> ImportAsync(NpgsqlConnection conn, string sourceDir)
    {
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                DROP TABLE IF EXISTS lahman.pitching;
                CREATE TABLE lahman.pitching (
                    player_id VARCHAR(9), year_id INT, stint INT,
                    team_id VARCHAR(3), lg_id VARCHAR(3),
                    w INT, l INT, g INT, gs INT, cg INT, sho INT, sv INT,
                    ip_outs INT, h INT, er INT, hr INT, bb INT, so INT,
                    era DECIMAL(5,2),
                    PRIMARY KEY (player_id, year_id, stint)
                );
                CREATE INDEX idx_pitching_player_year ON lahman.pitching (player_id, year_id);
                """;
            await cmd.ExecuteNonQueryAsync();
        }

        var csvPath = Path.Combine(sourceDir, "Pitching.csv");
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            { HasHeaderRecord = true, MissingFieldFound = null, BadDataFound = null });

        var records = csv.GetRecords<dynamic>().ToList();

        await using var writer = conn.BeginBinaryImport(
            "COPY lahman.pitching (player_id,year_id,stint,team_id,lg_id,w,l,g,gs,cg,sho,sv,ip_outs,h,er,hr,bb,so,era) FROM STDIN (FORMAT BINARY)");

        foreach (var r in records)
        {
            var d = (IDictionary<string, object>)r;
            await writer.StartRowAsync();
            Write(writer, S(d,"playerID")); Write(writer, I(d,"yearID")); Write(writer, I(d,"stint"));
            Write(writer, S(d,"teamID")); Write(writer, S(d,"lgID"));
            Write(writer, I(d,"W")); Write(writer, I(d,"L")); Write(writer, I(d,"G")); Write(writer, I(d,"GS"));
            Write(writer, I(d,"CG")); Write(writer, I(d,"SHO")); Write(writer, I(d,"SV"));
            Write(writer, I(d,"IPouts")); Write(writer, I(d,"H")); Write(writer, I(d,"ER"));
            Write(writer, I(d,"HR")); Write(writer, I(d,"BB")); Write(writer, I(d,"SO"));
            Write(writer, Dec(d,"ERA"));
        }

        await writer.CompleteAsync();
        return records.Count;
    }

    private static void Write(NpgsqlBinaryImporter w, object? v)
    {
        if (v is null) w.WriteNull();
        else if (v is string s) w.Write(s, NpgsqlTypes.NpgsqlDbType.Varchar);
        else if (v is int i) w.Write(i, NpgsqlTypes.NpgsqlDbType.Integer);
        else if (v is decimal dec) w.Write(dec, NpgsqlTypes.NpgsqlDbType.Numeric);
    }

    private static string? S(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && v?.ToString() is { Length: > 0 } s ? s : null;
    private static int? I(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && int.TryParse(v?.ToString(), out var i) ? i : (int?)null;
    private static decimal? Dec(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && decimal.TryParse(v?.ToString(), out var dec) ? dec : (decimal?)null;
}
