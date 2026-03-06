using Stratsphere.Core.Entities;

namespace Stratsphere.Core.Interfaces;

public interface IStandingsRepository
{
    Task<IEnumerable<Standings>> GetBySeasonIdAsync(Guid seasonId);
    Task<Standings?> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId);
    Task UpsertAsync(Standings standings);
    Task SaveChangesAsync();
}
