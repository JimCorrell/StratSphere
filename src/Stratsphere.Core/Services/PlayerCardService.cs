using Stratsphere.Core.Entities;
using Stratsphere.Core.Entities.Lahman;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Core.Services;

public class PlayerCardStats
{
    public required PlayerCard Card { get; init; }
    public required LahmanPerson Person { get; init; }

    // Historical stats from Lahman (always present for valid cards)
    public LahmanBatting? HistoricalBatting { get; init; }
    public LahmanPitching? HistoricalPitching { get; init; }

    // Simulated stats for a specific league season (null if card not in this league)
    public SimBattingStats? SimBatting { get; init; }
    public SimPitchingStats? SimPitching { get; init; }

    public bool IsPitcher => Card.Position is "SP" or "RP";
}

public class PlayerCardService(ILahmanRepository lahmanRepo)
{
    public async Task<PlayerCardStats?> GetCardStatsAsync(
        PlayerCard card, SimBattingStats? simBatting, SimPitchingStats? simPitching)
    {
        var person = await lahmanRepo.GetPersonAsync(card.LahmanPlayerId);
        if (person is null) return null;

        var isPitcher = card.Position is "SP" or "RP";

        return new PlayerCardStats
        {
            Card = card,
            Person = person,
            HistoricalBatting  = isPitcher ? null : await lahmanRepo.GetBattingSeasonAsync(card.LahmanPlayerId, card.CardYear),
            HistoricalPitching = isPitcher ? await lahmanRepo.GetPitchingSeasonAsync(card.LahmanPlayerId, card.CardYear) : null,
            SimBatting  = simBatting,
            SimPitching = simPitching
        };
    }
}
