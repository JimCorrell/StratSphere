using StratSphere.Core.Entities;

namespace StratSphere.Core.Interfaces;

public interface IRosterRepository
{
    Task<IEnumerable<RosterSlot>> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId);
    /// <summary>True if the card is currently active on any roster this season (DroppedAt is null).</summary>
    Task<bool> CardIsRosteredInSeasonAsync(Guid cardId, Guid seasonId);
    Task AddAsync(RosterSlot slot);
    /// <summary>Soft-delete: sets DroppedAt to now. Validates slot belongs to teamId before dropping.</summary>
    Task DropAsync(Guid slotId, Guid teamId);
    Task SaveChangesAsync();
}
