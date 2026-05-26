using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StratSphere.Core.Entities;
using StratSphere.Core.Interfaces;
using StratSphere.Web.Filters;
using StratSphere.Web.Middleware;
using StratSphere.Web.Models.ViewModels.Player;

namespace StratSphere.Web.Controllers;

[Authorize]
[LeagueMember]
public class PlayerController(ILahmanRepository lahmanRepo) : Controller
{
    // GET /league/{leagueAbbr}/player/{lahmanPlayerId}/{cardYear:int}
    public async Task<IActionResult> Detail(string lahmanPlayerId, int cardYear)
    {
        var league = HttpContext.Items[LeagueContextMiddleware.LeagueKey] as League;
        if (league is null) return NotFound();

        var person = await lahmanRepo.GetPersonAsync(lahmanPlayerId);
        if (person is null) return NotFound();

        // Load career data and card-year fielding in parallel
        var battingTask  = lahmanRepo.GetBattingAsync(lahmanPlayerId);
        var pitchingTask = lahmanRepo.GetPitchingAsync(lahmanPlayerId);
        var fieldingTask = lahmanRepo.GetFieldingSeasonAsync(lahmanPlayerId, cardYear);
        await Task.WhenAll(battingTask, pitchingTask, fieldingTask);

        var allBatting  = (await battingTask).ToList();
        var allPitching = (await pitchingTask).ToList();
        var fielding    = (await fieldingTask).ToList();

        // Determine position for this card year
        var cardYearPitching = allPitching.Where(p => p.YearId == cardYear).ToList();
        string cardPosition;
        if (cardYearPitching.Sum(p => p.IPOuts ?? 0) > 0)
        {
            var totalGS = cardYearPitching.Sum(p => p.GS ?? 0);
            var totalG  = cardYearPitching.Sum(p => p.G  ?? 0);
            cardPosition = totalGS * 2 >= totalG ? "SP" : "RP";
        }
        else
        {
            cardPosition = fielding
                .OrderByDescending(f => f.G)
                .Select(f => f.Pos)
                .FirstOrDefault() ?? "—";
        }

        // Aggregate by year (stints summed; multi-stint team label = "N teams")
        var careerBatting = allBatting
            .Where(b => (b.AB ?? 0) > 0 || (b.G ?? 0) > 0)
            .GroupBy(b => b.YearId)
            .OrderByDescending(g => g.Key)
            .Select(g => new PlayerDetailViewModel.BattingSeasonRow
            {
                YearId  = g.Key,
                TeamId  = g.Count() == 1 ? g.First().TeamId ?? "—" : $"{g.Count()} teams",
                LgId    = g.Count() == 1 ? g.First().LgId : null,
                G       = g.Sum(b => b.G       ?? 0),
                AB      = g.Sum(b => b.AB      ?? 0),
                R       = g.Sum(b => b.R       ?? 0),
                H       = g.Sum(b => b.H       ?? 0),
                Doubles = g.Sum(b => b.Doubles ?? 0),
                Triples = g.Sum(b => b.Triples ?? 0),
                HR      = g.Sum(b => b.HR      ?? 0),
                RBI     = g.Sum(b => b.RBI     ?? 0),
                BB      = g.Sum(b => b.BB      ?? 0),
                SO      = g.Sum(b => b.SO      ?? 0),
                SB      = g.Sum(b => b.SB      ?? 0),
                HBP     = g.Sum(b => b.HBP     ?? 0),
                SF      = g.Sum(b => b.SF      ?? 0),
            })
            .ToList();

        var careerPitching = allPitching
            .Where(p => (p.IPOuts ?? 0) > 0)
            .GroupBy(p => p.YearId)
            .OrderByDescending(g => g.Key)
            .Select(g => new PlayerDetailViewModel.PitchingSeasonRow
            {
                YearId = g.Key,
                TeamId = g.Count() == 1 ? g.First().TeamId ?? "—" : $"{g.Count()} teams",
                LgId   = g.Count() == 1 ? g.First().LgId : null,
                W      = g.Sum(p => p.W      ?? 0),
                L      = g.Sum(p => p.L      ?? 0),
                G      = g.Sum(p => p.G      ?? 0),
                GS     = g.Sum(p => p.GS     ?? 0),
                SV     = g.Sum(p => p.SV     ?? 0),
                IPOuts = g.Sum(p => p.IPOuts ?? 0),
                H      = g.Sum(p => p.H      ?? 0),
                ER     = g.Sum(p => p.ER     ?? 0),
                BB     = g.Sum(p => p.BB     ?? 0),
                SO     = g.Sum(p => p.SO     ?? 0),
            })
            .ToList();

        ViewData["ActiveTab"] = "players";

        return View(new PlayerDetailViewModel
        {
            LahmanPlayerId   = lahmanPlayerId,
            FullName         = person.FullName,
            CardPosition     = cardPosition,
            Bats             = person.Bats,
            Throws           = person.Throws,
            CardYear         = cardYear,
            BirthYear        = person.BirthYear,
            Debut            = person.Debut,
            FinalGame        = person.FinalGame,
            LeagueName       = league.Name,
            LeagueAbbreviation = league.Abbreviation,
            CareerBatting    = careerBatting,
            CareerPitching   = careerPitching,
        });
    }
}
