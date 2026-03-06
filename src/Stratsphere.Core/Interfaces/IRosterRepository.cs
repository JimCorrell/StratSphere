using Stratsphere.Core.Entities;

namespace Stratsphere.Core.Interfaces;

public interface IRosterRepository
{
    Task<IEnumerable<RosterSlot>> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId);
    Task<bool> CardIsRosteredInSeasonAsync(Guid cardId, Guid seasonId);
    Task AddAsync(RosterSlot slot);
    Task RemoveAsync(Guid slotId);
    Task SaveChangesAsync();
}
