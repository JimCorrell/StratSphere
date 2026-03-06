using Stratsphere.Core.Entities;

namespace Stratsphere.Core.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetBySeasonIdAsync(Guid seasonId);
    Task<Team?> GetByUserAndLeagueAsync(Guid userId, Guid leagueId);
    Task AddAsync(Team team);
    Task SaveChangesAsync();
}
