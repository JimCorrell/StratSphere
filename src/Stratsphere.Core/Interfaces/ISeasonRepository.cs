using StratSphere.Core.Entities;

namespace StratSphere.Core.Interfaces;

public interface ISeasonRepository
{
    Task<Season?> GetByIdAsync(Guid id);
    Task<IEnumerable<Season>> GetByLeagueAsync(Guid leagueId);
    Task AddAsync(Season season);
    Task SaveChangesAsync();
}
