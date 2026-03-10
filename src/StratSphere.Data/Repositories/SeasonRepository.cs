using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class SeasonRepository(StratSphereDbContext db) : ISeasonRepository
{
    public Task<Season?> GetByIdAsync(Guid id) =>
        db.Seasons.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Season>> GetByLeagueAsync(Guid leagueId) =>
        await db.Seasons
            .Where(s => s.LeagueId == leagueId)
            .OrderByDescending(s => s.CardYear)
            .ThenBy(s => s.Name)
            .ToListAsync();

    public async Task AddAsync(Season season) => await db.Seasons.AddAsync(season);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
