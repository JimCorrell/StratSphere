using Stratsphere.Core.Entities;

namespace Stratsphere.Core.Interfaces;

public interface ILeagueRepository
{
    Task<League?> GetByIdAsync(Guid id);
    Task<League?> GetBySlugAsync(string slug);
    Task<IEnumerable<League>> GetByUserIdAsync(Guid userId);
    Task<bool> SlugExistsAsync(string slug);
    Task AddAsync(League league);
    Task SaveChangesAsync();
}
