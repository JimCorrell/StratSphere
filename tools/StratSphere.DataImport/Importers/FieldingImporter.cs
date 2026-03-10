using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using System.Globalization;

namespace StratSphere.DataImport.Importers;

public class FieldingImporter : IImporter
{
    public string TableName => "lahman.fielding";

    public async Task<int> ImportAsync(NpgsqlConnection conn, string sourceDir)
    {
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                DROP TABLE IF EXISTS lahman.fielding;
                CREATE TABLE lahman.fielding (
                    player_id VARCHAR(9), year_id INT, stint INT,
                    team_id VARCHAR(3), lg_id VARCHAR(3), pos VARCHAR(5),
                    g INT, gs INT, inn_outs INT, po INT, a INT, e INT, dp INT,
                    PRIMARY KEY (player_id, year_id, stint, pos)
                );
                CREATE INDEX idx_fielding_player_year ON lahman.fielding (player_id, year_id);
                """;
            await cmd.ExecuteNonQueryAsync();
        }

        var csvPath = Path.Combine(sourceDir, "Fielding.csv");
        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            { HasHeaderRecord = true, MissingFieldFound = null, BadDataFound = null });

        var records = csv.GetRecords<dynamic>().ToList();

        await using var writer = conn.BeginBinaryImport(
            "COPY lahman.fielding (player_id,year_id,stint,team_id,lg_id,pos,g,gs,inn_outs,po,a,e,dp) FROM STDIN (FORMAT BINARY)");

        foreach (var r in records)
        {
            var d = (IDictionary<string, object>)r;
            await writer.StartRowAsync();
            Write(writer, S(d,"playerID")); Write(writer, I(d,"yearID")); Write(writer, I(d,"stint"));
            Write(writer, S(d,"teamID")); Write(writer, S(d,"lgID")); Write(writer, S(d,"POS"));
            Write(writer, I(d,"G")); Write(writer, I(d,"GS")); Write(writer, I(d,"InnOuts"));
            Write(writer, I(d,"PO")); Write(writer, I(d,"A")); Write(writer, I(d,"E")); Write(writer, I(d,"DP"));
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
