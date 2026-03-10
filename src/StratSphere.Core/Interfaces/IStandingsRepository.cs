using StratSphere.Core.Entities;

namespace StratSphere.Core.Interfaces;

public interface IStandingsRepository
{
    Task<IEnumerable<Standings>> GetBySeasonIdAsync(Guid seasonId);
    Task<Standings?> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId);
    Task UpsertAsync(Standings standings);
    Task SaveChangesAsync();
}
