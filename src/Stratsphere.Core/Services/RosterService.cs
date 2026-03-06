using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Core.Services;

public class RosterService(IRosterRepository rosterRepo, IPlayerCardRepository cardRepo)
{
    public async Task<RosterSlot> AddCardToRosterAsync(Guid teamId, Guid seasonId,
        string lahmanPlayerId, int cardYear, string position)
    {
        // Get or create the shared card definition
        var card = await cardRepo.GetOrCreateAsync(lahmanPlayerId, cardYear, position);

        // Enforce uniqueness: a card can only be on one team per season at a time
        if (await rosterRepo.CardIsRosteredInSeasonAsync(card.Id, seasonId))
            throw new InvalidOperationException(
                $"This card ({lahmanPlayerId} {cardYear}) is already on a roster this season.");

        var slot = new RosterSlot
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            SeasonId = seasonId,
            CardId = card.Id,
            SlotType = "active",
            AcquiredAt = DateTimeOffset.UtcNow
        };

        await rosterRepo.AddAsync(slot);
        await rosterRepo.SaveChangesAsync();
        return slot;
    }

    /// <summary>
    /// Drops a card from a roster (soft delete). Use for trades and releases.
    /// The slot is retained as trade/release history for the season.
    /// </summary>
    public async Task DropCardFromRosterAsync(Guid slotId)
    {
        await rosterRepo.DropAsync(slotId);
        await rosterRepo.SaveChangesAsync();
    }
}
