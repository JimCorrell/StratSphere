using System.Globalization;
using CsvHelper;
using Microsoft.Extensions.Logging;
using StratSphere.Core.Entities;
using StratSphere.Infrastructure.Data;

namespace StratSphere.Infrastructure.Services;

public interface IMlbImportService
{
    Task<ImportResult> ImportMlbDataAsync(string csvDirectoryPath, CancellationToken cancellationToken = default);
    Task<ImportResult> ImportTableAsync<T>(string csvFilePath, CancellationToken cancellationToken = default) where T : class;
}

public class ImportResult
{

    public TimeSpan Duration => EndTime - StartTime;
    public DateTime EndTime { get; set; }
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, int> RowsImportedByTable { get; set; } = new();
    public DateTime StartTime { get; set; }
    public bool Success => Errors.Count == 0;
    public int TotalTablesProcessed { get; set; }
}

public class MlbImportService : IMlbImportService
{
    private readonly StratSphereDbContext _context;
    private readonly ILogger<MlbImportService> _logger;

    public MlbImportService(StratSphereDbContext context, ILogger<MlbImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportResult> ImportMlbDataAsync(string csvDirectoryPath, CancellationToken cancellationToken = default)
    {
        var result = new ImportResult { StartTime = DateTime.UtcNow };

        if (!Directory.Exists(csvDirectoryPath))
        {
            result.Errors.Add($"Directory not found: {csvDirectoryPath}");
            result.EndTime = DateTime.UtcNow;
            return result;
        }

        var csvFiles = new Dictionary<string, Func<string, CancellationToken, Task<ImportResult>>>
        {
            { "People.csv", async (path, ct) => await ImportTableAsync<MlbPeople>(path, ct) },
            { "Teams.csv", async (path, ct) => await ImportTableAsync<MlbTeam>(path, ct) },
            { "Batting.csv", async (path, ct) => await ImportTableAsync<MlbBatting>(path, ct) },
            { "Pitching.csv", async (path, ct) => await ImportTableAsync<MlbPitching>(path, ct) },
            { "Fielding.csv", async (path, ct) => await ImportTableAsync<MlbFielding>(path, ct) },
            { "AllstarFull.csv", async (path, ct) => await ImportTableAsync<MlbAllstar>(path, ct) },
            { "HallOfFame.csv", async (path, ct) => await ImportTableAsync<MlbHallOfFame>(path, ct) },
        };

        foreach (var kvp in csvFiles)
        {
            var filePath = Path.Combine(csvDirectoryPath, kvp.Key);
            if (!File.Exists(filePath))
            {
                _logger.LogWarning("CSV file not found: {FilePath}", filePath);
                continue;
            }

            try
            {
                var importResult = await kvp.Value(filePath, cancellationToken);
                result.TotalTablesProcessed++;
                result.RowsImportedByTable[kvp.Key] = importResult.RowsImportedByTable.Values.FirstOrDefault();
                _logger.LogInformation("Successfully imported {FileName}: {RowCount} rows", kvp.Key, result.RowsImportedByTable[kvp.Key]);
            }
            catch (Exception ex)
            {
                var error = $"Failed to import {kvp.Key}: {ex.Message}";
                result.Errors.Add(error);
                _logger.LogError(ex, "Error importing {FileName}", kvp.Key);
            }
        }

        result.EndTime = DateTime.UtcNow;
        return result;
    }

    public async Task<ImportResult> ImportTableAsync<T>(string csvFilePath, CancellationToken cancellationToken = default) where T : class
    {
        var result = new ImportResult { StartTime = DateTime.UtcNow };
        var tableName = Path.GetFileNameWithoutExtension(csvFilePath);

        try
        {
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<T>().ToList();

                if (records.Count == 0)
                {
                    _logger.LogWarning("No records found in {FilePath}", csvFilePath);
                    result.EndTime = DateTime.UtcNow;
                    return result;
                }

                // Add records based on type
                if (typeof(T) == typeof(MlbPeople))
                {
                    var people = records.Cast<MlbPeople>().ToList();
                    await _context.MlbPeople.AddRangeAsync(people, cancellationToken);
                }
                else if (typeof(T) == typeof(MlbTeam))
                {
                    var teams = records.Cast<MlbTeam>().ToList();
                    await _context.MlbTeams.AddRangeAsync(teams, cancellationToken);
                }
                else if (typeof(T) == typeof(MlbBatting))
                {
                    var batters = records.Cast<MlbBatting>().ToList();
                    await _context.MlbBattings.AddRangeAsync(batters, cancellationToken);
                }
                else if (typeof(T) == typeof(MlbPitching))
                {
                    var pitchers = records.Cast<MlbPitching>().ToList();
                    await _context.MlbPitchings.AddRangeAsync(pitchers, cancellationToken);
                }
                else if (typeof(T) == typeof(MlbFielding))
                {
                    var fielders = records.Cast<MlbFielding>().ToList();
                    await _context.MlbFieldings.AddRangeAsync(fielders, cancellationToken);
                }
                else if (typeof(T) == typeof(MlbAllstar))
                {
                    var allstars = records.Cast<MlbAllstar>().ToList();
                    await _context.MlbAllstars.AddRangeAsync(allstars, cancellationToken);
                }
                else if (typeof(T) == typeof(MlbHallOfFame))
                {
                    var hofVotes = records.Cast<MlbHallOfFame>().ToList();
                    await _context.MlbHallOfFames.AddRangeAsync(hofVotes, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                result.RowsImportedByTable[tableName] = records.Count;
                _logger.LogInformation("Imported {Count} records from {TableName}", records.Count, tableName);
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Error importing {tableName}: {ex.Message}");
            _logger.LogError(ex, "Error importing table {TableName}", tableName);
        }

        result.EndTime = DateTime.UtcNow;
        return result;
    }
}
