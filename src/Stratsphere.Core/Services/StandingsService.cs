using Stratsphere.Core.Entities;
using Stratsphere.Core.Enums;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Core.Services;

public class StandingsService(IStandingsRepository standingsRepo)
{
    /// <summary>
    /// Recalculates standings for a season from scratch based on all completed games.
    /// Called after any game result is entered or corrected.
    /// </summary>
    public async Task RecalculateAsync(Guid seasonId, IEnumerable<Game> completedGames, IEnumerable<Team> teams)
    {
        // Initialize standings rows for all teams
        var map = teams.ToDictionary(t => t.Id, t => new Standings
        {
            Id = Guid.NewGuid(),
            TeamId = t.Id,
            SeasonId = seasonId
        });

        // Accumulate from completed games
        foreach (var game in completedGames.Where(g => g.Status == GameStatus.Completed))
        {
            if (!map.TryGetValue(game.HomeTeamId, out var home) ||
                !map.TryGetValue(game.AwayTeamId, out var away)) continue;

            home.RunsScored  += game.HomeScore ?? 0;
            home.RunsAllowed += game.AwayScore ?? 0;
            away.RunsScored  += game.AwayScore ?? 0;
            away.RunsAllowed += game.HomeScore ?? 0;

            if (game.HomeScore > game.AwayScore)      { home.Wins++;   away.Losses++; }
            else if (game.AwayScore > game.HomeScore) { away.Wins++;   home.Losses++; }
            else                                       { home.Ties++;   away.Ties++; }
        }

        foreach (var standings in map.Values)
        {
            standings.LastUpdated = DateTime.UtcNow;
            await standingsRepo.UpsertAsync(standings);
        }

        await standingsRepo.SaveChangesAsync();
    }
}
