using Microsoft.EntityFrameworkCore;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Data.Repositories;

public class RosterRepository(StratosphereDbContext db) : IRosterRepository
{
    public async Task<IEnumerable<RosterSlot>> GetByTeamAndSeasonAsync(Guid teamId, Guid seasonId) =>
        await db.RosterSlots
            .Include(r => r.Card)
            .Where(r => r.TeamId == teamId && r.SeasonId == seasonId)
            .OrderBy(r => r.Card.Position)
            .ToListAsync();

    public Task<bool> CardIsRosteredInSeasonAsync(Guid cardId, Guid seasonId) =>
        db.RosterSlots.AnyAsync(r => r.CardId == cardId && r.SeasonId == seasonId);

    public async Task AddAsync(RosterSlot slot) => await db.RosterSlots.AddAsync(slot);

    public async Task RemoveAsync(Guid slotId)
    {
        var slot = await db.RosterSlots.FindAsync(slotId);
        if (slot is not null) db.RosterSlots.Remove(slot);
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
