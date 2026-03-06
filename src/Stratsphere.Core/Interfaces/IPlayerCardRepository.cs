using Stratsphere.Core.Entities;

namespace Stratsphere.Core.Interfaces;

public interface IPlayerCardRepository
{
    Task<PlayerCard?> GetByIdAsync(Guid id);
    Task<PlayerCard?> GetByLahmanAsync(string lahmanPlayerId, int cardYear, string position);
    Task<PlayerCard> GetOrCreateAsync(string lahmanPlayerId, int cardYear, string position);
    Task SaveChangesAsync();
}
