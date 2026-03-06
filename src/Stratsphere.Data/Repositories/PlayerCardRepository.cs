using Microsoft.EntityFrameworkCore;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Data.Repositories;

public class PlayerCardRepository(StratosphereDbContext db) : IPlayerCardRepository
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
        return card;
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
