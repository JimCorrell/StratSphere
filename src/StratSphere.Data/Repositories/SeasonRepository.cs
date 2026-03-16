using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class SeasonRepository(StratSphereDbContext db) : ISeasonRepository
{
    public Task<Season?> GetByIdAsync(Guid id) =>
        db.Seasons.FirstOrDefaultAsync(s => s.Id == id);

    public Task<Season?> GetByLeagueAndYearAsync(Guid leagueId, int year) =>
        db.Seasons.FirstOrDefaultAsync(s => s.LeagueId == leagueId && s.CardYear == year);

    public async Task<IEnumerable<Season>> GetByLeagueAsync(Guid leagueId) =>
        await db.Seasons
            .Where(s => s.LeagueId == leagueId)
            .OrderByDescending(s => s.CardYear)
            .ThenBy(s => s.Name)
            .ToListAsync();

    public async Task AddAsync(Season season) => await db.Seasons.AddAsync(season);

    public async Task UpdateStatusAsync(Guid id, SeasonStatus status)
    {
        var season = await db.Seasons.FindAsync(id);
        if (season is not null)
        {
            season.Status = status;
            await db.SaveChangesAsync();
        }
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
