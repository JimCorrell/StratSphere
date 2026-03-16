using StratSphere.Core.Entities;

namespace StratSphere.Core.Interfaces;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id);
    Task<IEnumerable<Team>> GetBySeasonIdAsync(Guid seasonId);
    Task<bool> AbbreviationExistsInLeagueAsync(Guid leagueId, string abbreviation);
    Task<Team?> GetByUserAndLeagueAsync(Guid userId, Guid leagueId);
    Task AddAsync(Team team);
    Task SaveChangesAsync();
    Task<bool> ClaimAsync(Guid teamId, Guid userId, Guid leagueId);
}
