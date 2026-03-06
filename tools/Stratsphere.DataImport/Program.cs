using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Stratsphere.DataImport.Importers;

// ── Config ────────────────────────────────────────────────────────────────────
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddCommandLine(args)
    .Build();

var sourceDir  = config["source"]     ?? throw new Exception("--source <path> is required");
var connString = config["connection"] ?? throw new Exception("--connection <connstr> is required");

Console.WriteLine($"Stratsphere DataImport");
Console.WriteLine($"Source : {sourceDir}");
Console.WriteLine($"Target : {connString.Split(';').FirstOrDefault()}");
Console.WriteLine();

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

// Ensure lahman schema exists
await using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "CREATE SCHEMA IF NOT EXISTS lahman;";
    await cmd.ExecuteNonQueryAsync();
}

// ── Run importers ─────────────────────────────────────────────────────────────
var importers = new List<IImporter>
{
    new PeopleImporter(),
    new BattingImporter(),
    new PitchingImporter(),
    new FieldingImporter()
};

foreach (var importer in importers)
{
    Console.Write($"  Importing {importer.TableName}... ");
    var count = await importer.ImportAsync(conn, sourceDir);
    Console.WriteLine($"{count:N0} rows");
}

Console.WriteLine("\nDone.");
