using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class RosterRepository(StratSphereDbContext db) : IRosterRepository
{
    public async Task<IEnumerable<RosterSlot>> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId) =>
        await db.RosterSlots
            .Include(r => r.Card)
            .Where(r => r.TeamId == teamId && r.SeasonId == seasonId && r.DroppedAt == null)
            .OrderBy(r => r.Card.Position)
            .ToListAsync();

    public Task<bool> CardIsRosteredInSeasonAsync(Guid cardId, Guid seasonId) =>
        db.RosterSlots.AnyAsync(r => r.CardId == cardId && r.SeasonId == seasonId && r.DroppedAt == null);

    public async Task AddAsync(RosterSlot slot) => await db.RosterSlots.AddAsync(slot);

    public async Task DropAsync(Guid slotId, Guid teamId)
    {
        var slot = await db.RosterSlots.FirstOrDefaultAsync(r => r.Id == slotId && r.TeamId == teamId);
        if (slot is not null) slot.DroppedAt = DateTimeOffset.UtcNow;
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
