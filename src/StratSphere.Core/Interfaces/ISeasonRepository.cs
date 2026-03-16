using StratSphere.Core.Entities;
using StratSphere.Core.Enums;

namespace StratSphere.Core.Interfaces;

public interface ISeasonRepository
{
    Task<Season?> GetByIdAsync(Guid id);
    Task<Season?> GetByLeagueAndYearAsync(Guid leagueId, int year);
    Task<IEnumerable<Season>> GetByLeagueAsync(Guid leagueId);
    Task AddAsync(Season season);
    Task UpdateStatusAsync(Guid id, SeasonStatus status);
    Task SaveChangesAsync();
}
