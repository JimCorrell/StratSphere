using Microsoft.EntityFrameworkCore;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Data.Repositories;

public class StandingsRepository(StratosphereDbContext db) : IStandingsRepository
{
    public async Task<IEnumerable<Standings>> GetBySeasonIdAsync(Guid seasonId) =>
        await db.Standings
            .Include(s => s.Team)
            .Where(s => s.SeasonId == seasonId)
            .OrderByDescending(s => s.Wins)
            .ThenBy(s => s.Losses)
            .ToListAsync();

    public Task<Standings?> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId) =>
        db.Standings.FirstOrDefaultAsync(s => s.TeamId == teamId && s.SeasonId == seasonId);

    public async Task UpsertAsync(Standings standings)
    {
        var existing = await GetByTeamAndSeasonAsync(standings.TeamId, standings.SeasonId);
        if (existing is null)
            db.Standings.Add(standings);
        else
        {
            existing.Wins = standings.Wins;
            existing.Losses = standings.Losses;
            existing.Ties = standings.Ties;
            existing.RunsScored = standings.RunsScored;
            existing.RunsAllowed = standings.RunsAllowed;
            existing.Streak = standings.Streak;
            existing.LastUpdated = standings.LastUpdated;
        }
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
