using Microsoft.EntityFrameworkCore;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Data.Repositories;

public class TeamRepository(StratosphereDbContext db) : ITeamRepository
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
}
