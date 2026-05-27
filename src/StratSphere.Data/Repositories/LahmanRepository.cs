using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities.Lahman;
using StratSphere.Core.Interfaces;

namespace StratSphere.Data.Repositories;

public class LahmanRepository(StratSphereDbContext db) : ILahmanRepository
{
    public Task<LahmanPerson?> GetPersonAsync(string playerId) =>
        db.LahmanPeople.AsNoTracking().FirstOrDefaultAsync(p => p.PlayerId == playerId);

    public async Task<IReadOnlyDictionary<string, LahmanPerson>> GetPeopleAsync(IEnumerable<string> playerIds)
    {
        var ids = playerIds.ToList();
        return await db.LahmanPeople
            .AsNoTracking()
            .Where(p => ids.Contains(p.PlayerId))
            .ToDictionaryAsync(p => p.PlayerId);
    }

    public async Task<IEnumerable<LahmanPerson>> SearchPeopleAsync(string query, int limit = 20)
    {
        var q = query.ToLower();
        return await db.LahmanPeople
            .AsNoTracking()
            .Where(p => (p.NameFirst + " " + p.NameLast).ToLower().Contains(q)
                     || p.NameLast!.ToLower().StartsWith(q))
            .OrderBy(p => p.NameLast)
            .ThenBy(p => p.NameFirst)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<LahmanBatting>> GetBattingAsync(string playerId) =>
        await db.LahmanBatting
            .AsNoTracking()
            .Where(b => b.PlayerId == playerId)
            .OrderByDescending(b => b.YearId)
            .ToListAsync();

    public Task<LahmanBatting?> GetBattingSeasonAsync(string playerId, int yearId) =>
        db.LahmanBatting
            .AsNoTracking()
            .Where(b => b.PlayerId == playerId && b.YearId == yearId && b.Stint == 1)
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyDictionary<(string PlayerId, int Year), LahmanBatting>> GetBattingSeasonsAsync(
        IEnumerable<string> playerIds, IEnumerable<int> years)
    {
        var ids = playerIds.ToList();
        var yearList = years.ToList();
        var rows = await db.LahmanBatting
            .AsNoTracking()
            .Where(b => ids.Contains(b.PlayerId) && yearList.Contains(b.YearId) && b.Stint == 1)
            .ToListAsync();
        return rows.GroupBy(b => (b.PlayerId, b.YearId))
                   .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task<IEnumerable<LahmanPitching>> GetPitchingAsync(string playerId) =>
        await db.LahmanPitching
            .AsNoTracking()
            .Where(p => p.PlayerId == playerId)
            .OrderByDescending(p => p.YearId)
            .ToListAsync();

    public Task<LahmanPitching?> GetPitchingSeasonAsync(string playerId, int yearId) =>
        db.LahmanPitching
            .AsNoTracking()
            .Where(p => p.PlayerId == playerId && p.YearId == yearId && p.Stint == 1)
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyDictionary<(string PlayerId, int Year), LahmanPitching>> GetPitchingSeasonsAsync(
        IEnumerable<string> playerIds, IEnumerable<int> years)
    {
        var ids = playerIds.ToList();
        var yearList = years.ToList();
        var rows = await db.LahmanPitching
            .AsNoTracking()
            .Where(p => ids.Contains(p.PlayerId) && yearList.Contains(p.YearId) && p.Stint == 1)
            .ToListAsync();
        return rows.GroupBy(p => (p.PlayerId, p.YearId))
                   .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task<IEnumerable<LahmanFielding>> GetFieldingSeasonAsync(string playerId, int yearId) =>
        await db.LahmanFielding
            .AsNoTracking()
            .Where(f => f.PlayerId == playerId && f.YearId == yearId)
            .ToListAsync();

    public async Task<string?> FindPlayerIdAsync(string lastName, char firstInitial, int year, bool isPitcher)
    {
        var firstStr = firstInitial.ToString();

        // Strip generational suffixes that Lahman often omits (e.g., "Tatis Jr" → "Tatis")
        var cleanLast = System.Text.RegularExpressions.Regex
            .Replace(lastName, @"\s+(Jr\.?|Sr\.?|II|III|IV|V)$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            .Trim();

        // Try exact last-name match first, then fall back to prefix match
        var candidates = await db.LahmanPeople
            .AsNoTracking()
            .Where(p => p.NameLast != null &&
                       (p.NameLast == cleanLast || p.NameLast.StartsWith(cleanLast)) &&
                        p.NameFirst != null && p.NameFirst.StartsWith(firstStr))
            .Select(p => p.PlayerId)
            .ToListAsync();

        if (candidates.Count == 0) return null;

        // Narrow by having a stat row in the given year
        List<string> inYear;
        if (isPitcher)
        {
            inYear = await db.LahmanPitching
                .AsNoTracking()
                .Where(p => candidates.Contains(p.PlayerId) && p.YearId == year)
                .Select(p => p.PlayerId)
                .Distinct()
                .ToListAsync();
        }
        else
        {
            inYear = await db.LahmanBatting
                .AsNoTracking()
                .Where(b => candidates.Contains(b.PlayerId) && b.YearId == year)
                .Select(b => b.PlayerId)
                .Distinct()
                .ToListAsync();
        }

        return inYear.Count == 1 ? inYear[0] : null;
    }

    public async Task<IEnumerable<(LahmanPerson Person, string PrimaryPosition)>> SearchCardsAsync(
        string nameQuery, int? cardYear, bool pitchersOnly = false, int limit = 20)
    {
        var q = nameQuery.ToLower();

        if (pitchersOnly)
        {
            var pitcherIds = db.LahmanPitching.AsNoTracking()
                .Where(p => cardYear == null || p.YearId == cardYear)
                .Where(p => p.GS > 0 || p.G > 3)
                .Select(p => p.PlayerId);

            var pitchers = await db.LahmanPeople.AsNoTracking()
                .Where(p => pitcherIds.Contains(p.PlayerId))
                .Where(p => (p.NameFirst + " " + p.NameLast).ToLower().Contains(q))
                .OrderBy(p => p.NameLast).ThenBy(p => p.NameFirst)
                .Take(limit)
                .ToListAsync();

            return pitchers.Select(p => (p, "SP"));
        }
        else
        {
            var batterIds = db.LahmanBatting.AsNoTracking()
                .Where(b => cardYear == null || b.YearId == cardYear)
                .Where(b => b.AB > 50)
                .Select(b => b.PlayerId);

            var batters = await db.LahmanPeople.AsNoTracking()
                .Where(p => batterIds.Contains(p.PlayerId))
                .Where(p => (p.NameFirst + " " + p.NameLast).ToLower().Contains(q))
                .OrderBy(p => p.NameLast).ThenBy(p => p.NameFirst)
                .Take(limit)
                .ToListAsync();

            // Primary position derived from fielding — fall back to "OF" if not found
            var batchPlayerIds = batters.Select(p => p.PlayerId).ToList();
            var fieldingRows = await db.LahmanFielding
                .AsNoTracking()
                .Where(f => batchPlayerIds.Contains(f.PlayerId) && (cardYear == null || f.YearId == cardYear))
                .ToListAsync();
            var positionByPlayer = fieldingRows
                .GroupBy(f => f.PlayerId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(f => f.G).First().Pos);

            return batters.Select(p => (p, positionByPlayer.GetValueOrDefault(p.PlayerId) ?? "OF"));
        }
    }
}
