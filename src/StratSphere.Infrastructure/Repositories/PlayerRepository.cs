using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Infrastructure.Data;

namespace StratSphere.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly StratSphereDbContext _context;

    public PlayerRepository(StratSphereDbContext context)
    {
        _context = context;
    }

    public async Task<List<MlbPeople>> GetHallOfFameAsync()
    {
        return await _context.MlbPeople
            .Join(_context.MlbHallOfFames,
                p => p.PlayerId,
                h => h.PlayerId,
                (p, h) => new { Person = p, HofData = h })
            .Where(x => x.HofData.YearElected.HasValue)
            .Select(x => x.Person)
            .OrderBy(p => p.NameLast)
            .ThenBy(p => p.NameFirst)
            .ToListAsync();
    }

    public async Task<List<MlbBatting>> GetPlayerBattingStatsAsync(string playerId, int? startYear = null, int? endYear = null)
    {
        var query = _context.MlbBattings.Where(b => b.PlayerId == playerId);

        if (startYear.HasValue)
            query = query.Where(b => b.Year >= startYear);

        if (endYear.HasValue)
            query = query.Where(b => b.Year <= endYear);

        return await query
            .OrderByDescending(b => b.Year)
            .ToListAsync();
    }

    public async Task<MlbPeople?> GetPlayerByIdAsync(int id)
    {
        return await _context.MlbPeople.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<MlbFielding>> GetPlayerFieldingStatsAsync(string playerId, int? startYear = null, int? endYear = null)
    {
        var query = _context.MlbFieldings.Where(f => f.PlayerId == playerId);

        if (startYear.HasValue)
            query = query.Where(f => f.Year >= startYear);

        if (endYear.HasValue)
            query = query.Where(f => f.Year <= endYear);

        return await query
            .OrderByDescending(f => f.Year)
            .ToListAsync();
    }

    public async Task<List<MlbPitching>> GetPlayerPitchingStatsAsync(string playerId, int? startYear = null, int? endYear = null)
    {
        var query = _context.MlbPitchings.Where(p => p.PlayerId == playerId);

        if (startYear.HasValue)
            query = query.Where(p => p.Year >= startYear);

        if (endYear.HasValue)
            query = query.Where(p => p.Year <= endYear);

        return await query
            .OrderByDescending(p => p.Year)
            .ToListAsync();
    }

    public async Task<List<MlbPeople>> GetPlayersByPositionAsync(string position, int limit = 50)
    {
        return await _context.MlbFieldings
            .Where(f => f.Pos == position)
            .GroupBy(f => f.PlayerId)
            .Select(g => g.First().PlayerId)
            .Join(_context.MlbPeople,
                playerId => playerId,
                person => person.PlayerId,
                (_, person) => person)
            .Take(limit)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<MlbPeople>> SearchPlayersAsync(string searchTerm, int limit = 50)
    {
        var lowerTerm = searchTerm.ToLower();
        return await _context.MlbPeople
            .Where(p => (p.NameFirst != null && p.NameFirst.ToLower().Contains(lowerTerm)) ||
                        (p.NameLast != null && p.NameLast.ToLower().Contains(lowerTerm)))
            .OrderBy(p => p.NameLast)
            .ThenBy(p => p.NameFirst)
            .Take(limit)
            .ToListAsync();
    }
}
