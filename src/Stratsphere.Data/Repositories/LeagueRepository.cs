using Microsoft.EntityFrameworkCore;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Data.Repositories;

public class LeagueRepository(StratosphereDbContext db) : ILeagueRepository
{
    public Task<League?> GetByIdAsync(Guid id) =>
        db.Leagues.Include(l => l.Members).FirstOrDefaultAsync(l => l.Id == id);

    public Task<League?> GetBySlugAsync(string slug) =>
        db.Leagues.Include(l => l.Members).FirstOrDefaultAsync(l => l.Slug == slug);

    public async Task<IEnumerable<League>> GetByUserIdAsync(Guid userId) =>
        await db.Leagues
            .Where(l => l.Members.Any(m => m.UserId == userId))
            .OrderBy(l => l.Name)
            .ToListAsync();

    public Task<bool> SlugExistsAsync(string slug) =>
        db.Leagues.AnyAsync(l => l.Slug == slug);

    public async Task AddAsync(League league) => await db.Leagues.AddAsync(league);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
