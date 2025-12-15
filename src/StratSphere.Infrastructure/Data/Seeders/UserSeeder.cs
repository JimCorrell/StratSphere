using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Infrastructure.Services;

namespace StratSphere.Infrastructure.Data.Seeders;

public interface IUserSeeder
{
    Task SeedDefaultUsersAsync();
}

public class UserSeeder : IUserSeeder
{
    private readonly StratSphereDbContext _context;
    private readonly IPasswordService _passwordService;

    public UserSeeder(StratSphereDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task SeedDefaultUsersAsync()
    {
        // Check if users already exist
        if (await _context.Users.AnyAsync())
        {
            return;
        }

        // Create admin user
        var adminUser = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = "admin@stratsphere.local",
            Username = "admin",
            DisplayName = "Admin User",
            PasswordHash = _passwordService.HashPassword("Admin123!"),
            IsActive = true,
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create regular user
        var usblUser = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Email = "usbl@stratsphere.local",
            Username = "usbl_manager",
            DisplayName = "USBL Manager",
            PasswordHash = _passwordService.HashPassword("Usbl123!"),
            IsActive = true,
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(adminUser);
        _context.Users.Add(usblUser);
        await _context.SaveChangesAsync();

        // Get or create USBL league
        var usblLeague = await _context.Leagues.FirstOrDefaultAsync(l => l.Slug == "usbl");
        if (usblLeague == null)
        {
            usblLeague = new League
            {
                Id = Guid.NewGuid(),
                Name = "USBL",
                Slug = "usbl",
                Description = "Ultra Strat Baseball League",
                MaxTeams = 12,
                RosterSize = 40,
                ActiveRosterSize = 25,
                CurrentSeason = 2025,
                CurrentPhase = 0,
                Status = 0,
                UseDH = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Leagues.Add(usblLeague);
            await _context.SaveChangesAsync();
        }

        // Add admin user as member to all leagues (read-only, can be admin of all)
        var adminLeagueMembership = new LeagueMember
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            LeagueId = usblLeague.Id,
            Role = LeagueRole.Commissioner,
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.LeagueMembers.Add(adminLeagueMembership);

        // Add USBL user as member to USBL league only
        var usblLeagueMembership = new LeagueMember
        {
            Id = Guid.NewGuid(),
            UserId = usblUser.Id,
            LeagueId = usblLeague.Id,
            Role = LeagueRole.Member,
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.LeagueMembers.Add(usblLeagueMembership);

        await _context.SaveChangesAsync();
    }
}
