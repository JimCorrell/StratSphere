using Stratsphere.Core.Entities;
using Stratsphere.Core.Enums;
using Stratsphere.Core.Interfaces;

namespace Stratsphere.Core.Services;

public class LeagueService(ILeagueRepository leagueRepo)
{
    public async Task<League> CreateLeagueAsync(string name, Guid commissionerId)
    {
        var slug = await GenerateUniqueSlugAsync(name);

        var league = new League
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            CommissionerId = commissionerId,
            Status = "setup"
        };

        league.Members.Add(new LeagueMember
        {
            LeagueId = league.Id,
            UserId = commissionerId,
            Role = LeagueRole.Commissioner
        });

        await leagueRepo.AddAsync(league);
        await leagueRepo.SaveChangesAsync();
        return league;
    }

    public async Task<LeagueMember> JoinLeagueAsync(Guid leagueId, Guid userId)
    {
        var league = await leagueRepo.GetByIdAsync(leagueId)
            ?? throw new InvalidOperationException("League not found.");

        var member = new LeagueMember
        {
            LeagueId = leagueId,
            UserId = userId,
            Role = LeagueRole.Manager
        };

        league.Members.Add(member);
        await leagueRepo.SaveChangesAsync();
        return member;
    }

    private async Task<string> GenerateUniqueSlugAsync(string name)
    {
        var baseSlug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "");

        // Keep only alphanumeric and hyphens
        baseSlug = new string(baseSlug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

        var slug = baseSlug;
        var counter = 1;
        while (await leagueRepo.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{counter++}";
        }
        return slug;
    }
}
