using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class LeagueRepository(StratSphereDbContext db) : ILeagueRepository
{
    public Task<League?> GetByIdAsync(Guid id) =>
        db.Leagues.Include(l => l.Members).FirstOrDefaultAsync(l => l.Id == id);

    public Task<League?> GetBySlugAsync(string slug) =>
        db.Leagues
            .Include(l => l.Members).ThenInclude(m => m.User)
            .Include(l => l.Teams).ThenInclude(t => t.User)
            .Include(l => l.Seasons)
            .FirstOrDefaultAsync(l => l.Slug == slug);

    public async Task<IEnumerable<League>> GetByUserIdAsync(Guid userId) =>
        await db.Leagues
            .Include(l => l.Members)
            .Where(l => l.Members.Any(m => m.UserId == userId))
            .OrderBy(l => l.Name)
            .ToListAsync();

    public Task<bool> SlugExistsAsync(string slug) =>
        db.Leagues.AnyAsync(l => l.Slug == slug);

    public async Task AddAsync(League league) => await db.Leagues.AddAsync(league);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
