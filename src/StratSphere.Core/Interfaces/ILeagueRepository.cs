using StratSphere.Core.Entities;

namespace StratSphere.Core.Interfaces;

public interface ILeagueRepository
{
    Task<League?> GetByIdAsync(Guid id);
    Task<League?> GetBySlugAsync(string slug);
    Task<League?> GetByAbbreviationAsync(string abbreviation);
    Task<IEnumerable<League>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<League>> GetAllAsync();
    Task<bool> SlugExistsAsync(string slug);
    Task<bool> AbbreviationExistsAsync(string abbreviation);
    Task AddAsync(League league);
    Task ArchiveAsync(Guid id);
    Task UnarchiveAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
    Task SaveChangesAsync();
}
