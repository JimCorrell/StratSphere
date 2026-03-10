using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class TeamRepository(StratSphereDbContext db) : ITeamRepository
{
    public Task<Team?> GetByIdAsync(Guid id) =>
        db.Teams.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Team>> GetBySeasonIdAsync(Guid seasonId) =>
        await db.Teams
            .Where(t => t.RosterSlots.Any(r => r.SeasonId == seasonId))
            .Include(t => t.User)
            .ToListAsync();

    public Task<Team?> GetByUserAndLeagueAsync(Guid userId, Guid leagueId) =>
        db.Teams.FirstOrDefaultAsync(t => t.UserId == userId && t.LeagueId == leagueId);

    public async Task AddAsync(Team team) => await db.Teams.AddAsync(team);
    public Task SaveChangesAsync() => db.SaveChangesAsync();

    public async Task<bool> ClaimAsync(Guid teamId, Guid userId, Guid leagueId)
    {
        var team = await db.Teams.FirstOrDefaultAsync(t =>
            t.Id == teamId && t.LeagueId == leagueId && t.UserId == null);
        if (team is null) return false;
        team.UserId = userId;
        await db.SaveChangesAsync();
        return true;
    }
}
