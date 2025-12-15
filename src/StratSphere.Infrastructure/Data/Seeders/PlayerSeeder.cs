using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;

namespace StratSphere.Infrastructure.Data.Seeders;

/// <summary>
/// Service to initialize the database.
/// MLB historical data is imported via the MlbImportService from CSV files.
/// </summary>
public interface IPlayerSeeder
{
    Task InitializeDatabaseAsync();
    Task SeedSamplePlayersAsync();
}

public class PlayerSeeder : IPlayerSeeder
{
    private readonly StratSphereDbContext _context;

    public PlayerSeeder(StratSphereDbContext context)
    {
        _context = context;
    }

    public async Task InitializeDatabaseAsync()
    {
        // Ensure database is created and migrations are applied
        await _context.Database.MigrateAsync();
    }

    public async Task SeedSamplePlayersAsync()
    {
        // MLB data is imported via MlbImportService from CSV files
        // No sample data seeding needed
        await Task.CompletedTask;
    }
}
