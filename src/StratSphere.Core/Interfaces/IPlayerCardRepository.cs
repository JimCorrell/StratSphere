using StratSphere.Core.Entities;

namespace StratSphere.Core.Interfaces;

public interface IPlayerCardRepository
{
    Task<PlayerCard?> GetByIdAsync(Guid id);
    Task<PlayerCard?> GetByLahmanAsync(string lahmanPlayerId, int cardYear, string position);

    /// <summary>Find any card for this player+year regardless of position. Used by import.</summary>
    Task<PlayerCard?> GetByLahmanAndYearAsync(string lahmanPlayerId, int cardYear);

    Task<PlayerCard> GetOrCreateAsync(string lahmanPlayerId, int cardYear, string position);
    Task SaveChangesAsync();
}
