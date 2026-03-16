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

    public Task<League?> GetByAbbreviationAsync(string abbreviation) =>
        db.Leagues
            .Include(l => l.Members).ThenInclude(m => m.User)
            .Include(l => l.Teams).ThenInclude(t => t.User)
            .Include(l => l.Seasons)
            .FirstOrDefaultAsync(l => l.Abbreviation.ToUpper() == abbreviation.ToUpper());

    public async Task<IEnumerable<League>> GetByUserIdAsync(Guid userId) =>
        await db.Leagues
            .Include(l => l.Members)
            .Where(l => l.Members.Any(m => m.UserId == userId) && l.ArchivedAt == null)
            .OrderBy(l => l.Name)
            .ToListAsync();

    public async Task<IEnumerable<League>> GetAllAsync() =>
        await db.Leagues
            .Include(l => l.Members)
            .OrderBy(l => l.Name)
            .ToListAsync();

    public Task<bool> SlugExistsAsync(string slug) =>
        db.Leagues.AnyAsync(l => l.Slug == slug);

    public Task<bool> AbbreviationExistsAsync(string abbreviation) =>
        db.Leagues.AnyAsync(l => l.Abbreviation == abbreviation);

    public async Task AddAsync(League league) => await db.Leagues.AddAsync(league);

    public async Task ArchiveAsync(Guid id)
    {
        var league = await db.Leagues.FindAsync(id);
        if (league is not null) { league.ArchivedAt = DateTime.UtcNow; await db.SaveChangesAsync(); }
    }

    public async Task UnarchiveAsync(Guid id)
    {
        var league = await db.Leagues.FindAsync(id);
        if (league is not null) { league.ArchivedAt = null; await db.SaveChangesAsync(); }
    }

    public async Task DeleteAsync(Guid id)
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DELETE FROM sim_batting_stats  WHERE season_id IN (SELECT id FROM seasons WHERE league_id = {0});
            DELETE FROM sim_pitching_stats WHERE season_id IN (SELECT id FROM seasons WHERE league_id = {0});
            DELETE FROM standings          WHERE season_id IN (SELECT id FROM seasons WHERE league_id = {0});
            DELETE FROM games              WHERE season_id IN (SELECT id FROM seasons WHERE league_id = {0});
            DELETE FROM roster_slots       WHERE season_id IN (SELECT id FROM seasons WHERE league_id = {0});
            DELETE FROM teams        WHERE league_id = {0};
            DELETE FROM league_members     WHERE league_id = {0};
            DELETE FROM seasons            WHERE league_id = {0};
            DELETE FROM leagues            WHERE id        = {0};
        ", id);
    }

    public async Task DeleteAllAsync()
    {
        await db.Database.ExecuteSqlRawAsync(@"
            DELETE FROM sim_batting_stats;
            DELETE FROM sim_pitching_stats;
            DELETE FROM standings;
            DELETE FROM games;
            DELETE FROM roster_slots;
            DELETE FROM teams;
            DELETE FROM league_members;
            DELETE FROM seasons;
            DELETE FROM leagues;
        ");
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
