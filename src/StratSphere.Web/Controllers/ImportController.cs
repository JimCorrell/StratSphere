using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Core.Enums;
using StratSphere.Core.Interfaces;
using StratSphere.Core.Services;
using StratSphere.Data;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using StratSphere.Web.Models.ViewModels.Import;
using StratSphere.Web.Services;
using System.Text.Json;
using IOFile = System.IO.File;

namespace StratSphere.Web.Controllers;

[Authorize]
[LeagueMember(CommissionerOnly = true)]
public class ImportController(
    StratSphereDbContext     db,
    ISeasonRepository        seasonRepo,
    ITeamRepository          teamRepo,
    ILahmanRepository        lahmanRepo,
    IPlayerCardRepository    cardRepo,
    StandingsService         standingsService,
    SomImportService         importService) : Controller
{
    private League ActiveLeague =>
        (HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League)!;

    // GET /league/{abbr}/import
    public async Task<IActionResult> Index()
    {
        var league = ActiveLeague;
        var season = await GetActiveSeasonAsync(league.Id);

        return View(new SomUploadViewModel
        {
            LeagueAbbreviation = league.Abbreviation,
            LeagueName         = league.Name,
            SeasonName         = season?.Name ?? "(no active season)"
        });
    }

    // POST /league/{abbr}/import/upload
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var league = ActiveLeague;
        var season = await GetActiveSeasonAsync(league.Id);

        if (season is null)
        {
            TempData["Error"] = "No active season found. Activate a season before importing.";
            return Redirect($"/league/{league.Abbreviation}/import");
        }

        if (file is null || file.Length == 0)
        {
            TempData["Error"] = "Please select a .lzp file to upload.";
            return Redirect($"/league/{league.Abbreviation}/import");
        }

        // Parse the archive
        SomParseResult parsed;
        try
        {
            using var stream = file.OpenReadStream();
            parsed = importService.Parse(stream);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Could not parse archive: {ex.Message}";
            return Redirect($"/league/{league.Abbreviation}/import");
        }

        if (parsed.TeamOrder.Length == 0)
        {
            TempData["Error"] = "Could not read team order from archive (no .H01 file found in Export/).";
            return Redirect($"/league/{league.Abbreviation}/import");
        }

        // Save parse result to temp file — too large for TempData cookie
        var key     = Guid.NewGuid().ToString("N");
        var tmpPath = Path.Combine(Path.GetTempPath(), $"som_{key}.json");
        await IOFile.WriteAllTextAsync(tmpPath, JsonSerializer.Serialize(parsed));
        TempData["SomTempKey"] = key;

        return Redirect($"/league/{league.Abbreviation}/import/map-teams");
    }

    // GET /league/{abbr}/import/map-teams
    public async Task<IActionResult> MapTeams()
    {
        var league = ActiveLeague;
        var season = await GetActiveSeasonAsync(league.Id);
        if (season is null) return Redirect($"/league/{league.Abbreviation}/import");

        var key = TempData.Peek("SomTempKey") as string;
        TempData.Keep("SomTempKey");
        if (key is null) return Redirect($"/league/{league.Abbreviation}/import");

        var tmpPath = Path.Combine(Path.GetTempPath(), $"som_{key}.json");
        if (!IOFile.Exists(tmpPath)) return Redirect($"/league/{league.Abbreviation}/import");

        var parsed   = JsonSerializer.Deserialize<SomParseResult>(await IOFile.ReadAllTextAsync(tmpPath))!;
        var ssTeams  = (await teamRepo.GetByLeagueIdAsync(league.Id)).ToList();

        // Auto-suggest: if SOM abbr matches a StratSphere team abbr (case-insensitive), pre-select
        var suggestions = parsed.TeamOrder
            .Select((abbr, idx) => (abbr, idx))
            .Where(t => ssTeams.Any(s => s.Abbreviation.Equals(t.abbr, StringComparison.OrdinalIgnoreCase)))
            .ToDictionary(
                t => t.abbr,
                t => ssTeams.First(s => s.Abbreviation.Equals(t.abbr, StringComparison.OrdinalIgnoreCase)).Id
            );

        return View(new SomMapTeamsViewModel
        {
            LeagueAbbreviation = league.Abbreviation,
            LeagueName         = league.Name,
            SeasonName         = season.Name,
            TempFileKey        = key,
            SomTeams           = parsed.TeamOrder,
            StratSphereTeams   = ssTeams.Select(t => new TeamOption
            {
                Id           = t.Id,
                Abbreviation = t.Abbreviation,
                DisplayName  = $"{t.City} {t.Name} ({t.Abbreviation})"
            }).OrderBy(t => t.DisplayName).ToList(),
            Suggestions        = suggestions
        });
    }

    // POST /league/{abbr}/import/run
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Run(string tempFileKey, Dictionary<string, string> teamMap)
    {
        var league = ActiveLeague;
        var season = await GetActiveSeasonAsync(league.Id);
        if (season is null)
        {
            TempData["Error"] = "No active season.";
            return Redirect($"/league/{league.Abbreviation}/import");
        }

        var tmpPath = Path.Combine(Path.GetTempPath(), $"som_{tempFileKey}.json");
        if (!IOFile.Exists(tmpPath))
        {
            TempData["Error"] = "Import session expired. Please upload the file again.";
            return Redirect($"/league/{league.Abbreviation}/import");
        }

        var parsed   = JsonSerializer.Deserialize<SomParseResult>(await IOFile.ReadAllTextAsync(tmpPath))!;
        var ssTeams  = (await teamRepo.GetByLeagueIdAsync(league.Id)).ToDictionary(t => t.Id);

        // Build index → StratSphere Team mapping
        // teamMap keys are SOM team abbreviations, values are StratSphere team ID strings
        var indexToTeam = new Dictionary<int, Team>();
        for (int i = 0; i < parsed.TeamOrder.Length; i++)
        {
            var somAbbr = parsed.TeamOrder[i];
            if (teamMap.TryGetValue(somAbbr, out var idStr) &&
                Guid.TryParse(idStr, out var teamId) &&
                ssTeams.TryGetValue(teamId, out var team))
            {
                indexToTeam[i] = team;
            }
        }

        int gamesImported    = 0;
        int battersImported  = 0;
        int pitchersImported = 0;
        int unmatched        = 0;

        // ── Games ──────────────────────────────────────────────────────────────
        var baseDate = season.StartDate ?? new DateOnly(season.CardYear, 4, 1);

        foreach (var game in parsed.Games)
        {
            if (!indexToTeam.TryGetValue(game.HomeTeam, out var homeTeam) ||
                !indexToTeam.TryGetValue(game.AwayTeam, out var awayTeam)) continue;

            var gameDate = baseDate.AddDays(game.Day);

            var existing = await db.Games
                .FirstOrDefaultAsync(g => g.SeasonId     == season.Id
                                       && g.HomeTeamId   == homeTeam.Id
                                       && g.AwayTeamId   == awayTeam.Id
                                       && g.GameDate     == gameDate);
            if (existing is null)
            {
                db.Games.Add(new Game
                {
                    Id         = Guid.NewGuid(),
                    SeasonId   = season.Id,
                    HomeTeamId = homeTeam.Id,
                    AwayTeamId = awayTeam.Id,
                    HomeScore  = game.HomeRuns,
                    AwayScore  = game.AwayRuns,
                    GameDate   = gameDate,
                    Status     = GameStatus.Completed
                });
                gamesImported++;
            }
            else if (existing.Status != GameStatus.Completed)
            {
                existing.HomeScore = game.HomeRuns;
                existing.AwayScore = game.AwayRuns;
                existing.Status    = GameStatus.Completed;
            }
        }
        await db.SaveChangesAsync();

        // ── Batting stats ──────────────────────────────────────────────────────
        foreach (var b in parsed.Batting)
        {
            if (!indexToTeam.TryGetValue(b.PrimaryTeamIndex, out var team)) continue;

            var lahmanId = await lahmanRepo.FindPlayerIdAsync(b.LastName, b.FirstInitial, b.CardYear, isPitcher: false);
            if (lahmanId is null) { unmatched++; continue; }

            // Prefer existing card for this player+year (any position) to avoid duplicates
            var card = await cardRepo.GetByLahmanAndYearAsync(lahmanId, b.CardYear)
                    ?? await cardRepo.GetOrCreateAsync(lahmanId, b.CardYear, "OF");

            var stats = await db.SimBattingStats
                .FirstOrDefaultAsync(s => s.CardId == card.Id && s.SeasonId == season.Id);

            if (stats is null)
            {
                db.SimBattingStats.Add(new SimBattingStats
                {
                    Id       = Guid.NewGuid(),
                    CardId   = card.Id,
                    SeasonId = season.Id,
                    TeamId   = team.Id,
                    G = b.G, AB = b.AB, H = b.H, BB = b.BB, SO = b.SO,
                    HR = b.HR, R = b.R, RBI = b.RBI, SB = b.SB
                });
            }
            else
            {
                stats.TeamId = team.Id;
                stats.G  = b.G; stats.AB = b.AB; stats.H  = b.H;
                stats.BB = b.BB; stats.SO = b.SO; stats.HR = b.HR;
                stats.R  = b.R;  stats.RBI = b.RBI; stats.SB = b.SB;
            }
            battersImported++;
        }
        await db.SaveChangesAsync();

        // ── Pitching stats ─────────────────────────────────────────────────────
        foreach (var p in parsed.Pitching)
        {
            if (!indexToTeam.TryGetValue(p.PrimaryTeamIndex, out var team)) continue;

            var lahmanId = await lahmanRepo.FindPlayerIdAsync(p.LastName, p.FirstInitial, p.CardYear, isPitcher: true);
            if (lahmanId is null) { unmatched++; continue; }

            // Derive position from GS ratio: mostly-starter → SP, otherwise RP
            var pitcherPos = (p.G > 0 && p.GS * 2 >= p.G) ? "SP" : "RP";
            var card = await cardRepo.GetByLahmanAndYearAsync(lahmanId, p.CardYear)
                    ?? await cardRepo.GetOrCreateAsync(lahmanId, p.CardYear, pitcherPos);

            var stats = await db.SimPitchingStats
                .FirstOrDefaultAsync(s => s.CardId == card.Id && s.SeasonId == season.Id);

            if (stats is null)
            {
                db.SimPitchingStats.Add(new SimPitchingStats
                {
                    Id       = Guid.NewGuid(),
                    CardId   = card.Id,
                    SeasonId = season.Id,
                    TeamId   = team.Id,
                    W = p.W, L = p.L, G = p.G, GS = p.GS, SV = p.SV,
                    IPOuts = p.IPOuts, H = p.H, ER = p.ER, BB = p.BB, SO = p.SO, HR = p.HR
                });
            }
            else
            {
                stats.TeamId = team.Id;
                stats.W  = p.W; stats.L  = p.L; stats.G  = p.G;
                stats.GS = p.GS; stats.SV = p.SV;
                stats.IPOuts = p.IPOuts; stats.H  = p.H; stats.ER = p.ER;
                stats.BB = p.BB; stats.SO = p.SO; stats.HR = p.HR;
            }
            pitchersImported++;
        }
        await db.SaveChangesAsync();

        // ── Recalculate standings ──────────────────────────────────────────────
        var completedGames = await db.Games
            .Where(g => g.SeasonId == season.Id && g.Status == GameStatus.Completed)
            .ToListAsync();
        var allTeams = await teamRepo.GetBySeasonIdAsync(season.Id);
        await standingsService.RecalculateAsync(season.Id, completedGames, allTeams);

        // Cleanup temp file
        IOFile.Delete(tmpPath);
        TempData.Remove("SomTempKey");

        TempData["Success"] = $"Import complete — {gamesImported} games, {battersImported} batters, {pitchersImported} pitchers imported. {unmatched} players could not be matched to Lahman.";
        return Redirect($"/league/{league.Abbreviation}");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<Season?> GetActiveSeasonAsync(Guid leagueId)
    {
        var seasons = await seasonRepo.GetByLeagueAsync(leagueId);
        return seasons.FirstOrDefault(s => s.Status == SeasonStatus.Active);
    }
}
