using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class PlayerCardRepository(StratSphereDbContext db) : IPlayerCardRepository
{
    public Task<PlayerCard?> GetByIdAsync(Guid id) =>
        db.PlayerCards.FirstOrDefaultAsync(c => c.Id == id);

    public Task<PlayerCard?> GetByLahmanAsync(string lahmanPlayerId, int cardYear, string position) =>
        db.PlayerCards.FirstOrDefaultAsync(c =>
            c.LahmanPlayerId == lahmanPlayerId &&
            c.CardYear == cardYear &&
            c.Position == position);

    public async Task<PlayerCard> GetOrCreateAsync(string lahmanPlayerId, int cardYear, string position)
    {
        var card = await GetByLahmanAsync(lahmanPlayerId, cardYear, position);
        if (card is not null) return card;

        card = new PlayerCard
        {
            Id = Guid.NewGuid(),
            LahmanPlayerId = lahmanPlayerId,
            CardYear = cardYear,
            Position = position
        };
        db.PlayerCards.Add(card);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // A concurrent request created the same card — detach and re-fetch
            db.Entry(card).State = EntityState.Detached;
            card = await GetByLahmanAsync(lahmanPlayerId, cardYear, position)
                ?? throw new InvalidOperationException(
                    $"Failed to create or retrieve player card ({lahmanPlayerId}, {cardYear}, {position}).");
        }

        return card;
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
