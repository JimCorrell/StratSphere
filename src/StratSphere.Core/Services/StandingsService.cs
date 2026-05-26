using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;

namespace StratSphere.Core.Services;

public class StandingsService(IStandingsRepository standingsRepo)
{
    /// <summary>
    /// Recalculates standings for a season from scratch based on all completed games.
    /// Called after any game result is entered or corrected.
    /// </summary>
    public async Task RecalculateAsync(Guid seasonId, IEnumerable<Game> completedGames, IEnumerable<Team> teams)
    {
        var map = teams.ToDictionary(t => t.Id, t => new Standings
        {
            Id = Guid.NewGuid(),
            TeamId = t.Id,
            SeasonId = seasonId
        });

        // 'W', 'L', 'T' per team in chronological order — used for Last10 and Streak
        var resultLog = teams.ToDictionary(t => t.Id, _ => new List<char>());

        foreach (var game in completedGames
            .Where(g => g.Status == GameStatus.Completed)
            .OrderBy(g => g.GameDate))
        {
            if (!map.TryGetValue(game.HomeTeamId, out var home) ||
                !map.TryGetValue(game.AwayTeamId, out var away)) continue;

            home.RunsScored  += game.HomeScore ?? 0;
            home.RunsAllowed += game.AwayScore ?? 0;
            away.RunsScored  += game.AwayScore ?? 0;
            away.RunsAllowed += game.HomeScore ?? 0;

            if (game.HomeScore > game.AwayScore)
            {
                home.Wins++;   home.HomeWins++;
                away.Losses++; away.AwayLosses++;
                resultLog[game.HomeTeamId].Add('W');
                resultLog[game.AwayTeamId].Add('L');
            }
            else if (game.AwayScore > game.HomeScore)
            {
                away.Wins++;   away.AwayWins++;
                home.Losses++; home.HomeLosses++;
                resultLog[game.AwayTeamId].Add('W');
                resultLog[game.HomeTeamId].Add('L');
            }
            else
            {
                home.Ties++; away.Ties++;
                resultLog[game.HomeTeamId].Add('T');
                resultLog[game.AwayTeamId].Add('T');
            }
        }

        foreach (var (teamId, results) in resultLog)
        {
            if (!map.TryGetValue(teamId, out var s)) continue;

            var last10 = results.TakeLast(10).ToList();
            s.Last10Wins   = last10.Count(r => r == 'W');
            s.Last10Losses = last10.Count(r => r == 'L');

            if (results.Count > 0)
            {
                char last = results[^1];
                int  len  = results.AsEnumerable().Reverse().TakeWhile(r => r == last).Count();
                s.Streak = $"{last}{len}";
            }
        }

        foreach (var standings in map.Values)
        {
            standings.LastUpdated = DateTime.UtcNow;
            await standingsRepo.UpsertAsync(standings);
        }

        await standingsRepo.SaveChangesAsync();
    }
}
