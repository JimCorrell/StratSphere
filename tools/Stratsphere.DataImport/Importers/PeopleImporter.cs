using CsvHelper;
using CsvHelper.Configuration;
using Npgsql;
using System.Globalization;

namespace Stratsphere.DataImport.Importers;

public class PeopleImporter : IImporter
{
    public string TableName => "lahman.people";

    public async Task<int> ImportAsync(NpgsqlConnection conn, string sourceDir)
    {
        var csvPath = Path.Combine(sourceDir, "People.csv");

        // Recreate table
        await using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = """
                DROP TABLE IF EXISTS lahman.people;
                CREATE TABLE lahman.people (
                    player_id   VARCHAR(9)  PRIMARY KEY,
                    birth_year  INT,
                    birth_month INT,
                    birth_day   INT,
                    name_first  VARCHAR(255),
                    name_last   VARCHAR(255),
                    name_given  VARCHAR(255),
                    weight      INT,
                    height      INT,
                    bats        VARCHAR(1),
                    throws      VARCHAR(1),
                    debut       DATE,
                    final_game  DATE,
                    bbref_id    VARCHAR(255),
                    retro_id    VARCHAR(255)
                );
                """;
            await cmd.ExecuteNonQueryAsync();
        }

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        });

        var records = csv.GetRecords<dynamic>().ToList();

        await using var writer = conn.BeginBinaryImport(
            "COPY lahman.people (player_id, birth_year, birth_month, birth_day, name_first, name_last, name_given, weight, height, bats, throws, debut, final_game, bbref_id, retro_id) FROM STDIN (FORMAT BINARY)");

        foreach (var r in records)
        {
            var dict = (IDictionary<string, object>)r;
            await writer.StartRowAsync();
            await writer.WriteAsync(S(dict, "playerID"),         NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(I(dict, "birthYear"),        NpgsqlTypes.NpgsqlDbType.Integer);
            await writer.WriteAsync(I(dict, "birthMonth"),       NpgsqlTypes.NpgsqlDbType.Integer);
            await writer.WriteAsync(I(dict, "birthDay"),         NpgsqlTypes.NpgsqlDbType.Integer);
            await writer.WriteAsync(S(dict, "nameFirst"),        NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(S(dict, "nameLast"),         NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(S(dict, "nameGiven"),        NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(I(dict, "weight"),           NpgsqlTypes.NpgsqlDbType.Integer);
            await writer.WriteAsync(I(dict, "height"),           NpgsqlTypes.NpgsqlDbType.Integer);
            await writer.WriteAsync(S(dict, "bats"),             NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(S(dict, "throws"),           NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(D(dict, "debut"),            NpgsqlTypes.NpgsqlDbType.Date);
            await writer.WriteAsync(D(dict, "finalGame"),        NpgsqlTypes.NpgsqlDbType.Date);
            await writer.WriteAsync(S(dict, "bbrefID"),          NpgsqlTypes.NpgsqlDbType.Varchar);
            await writer.WriteAsync(S(dict, "retroID"),          NpgsqlTypes.NpgsqlDbType.Varchar);
        }

        await writer.CompleteAsync();
        return records.Count;
    }

    private static string? S(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && v?.ToString() is { Length: > 0 } s ? s : null;

    private static int? I(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && int.TryParse(v?.ToString(), out var i) ? i : null;

    private static DateOnly? D(IDictionary<string, object> d, string key) =>
        d.TryGetValue(key, out var v) && DateOnly.TryParse(v?.ToString(), out var dt) ? dt : null;
}
