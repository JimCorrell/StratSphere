using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StratSphere.Core.Entities;
using StratSphere.Data;

namespace StratSphere.Web.Services;

/// <summary>
/// Seeds the super admin user on startup. On first run (no admin present),
/// wipes all app data and creates the protected super admin account.
/// The super admin is never created with a password that passes normal validation —
/// password hash is set directly to allow shorter/custom passwords for the owner account.
/// </summary>
public class DataSeeder(
    UserManager<ApplicationUser> userManager,
    StratSphereDbContext db,
    ILogger<DataSeeder> logger)
{
    private const string AdminEmail    = "jim.correll@gmail.com";
    private const string AdminPassword = "3mm@K@t3";
    private const string AdminDisplay  = "Super Admin";
    private const string AdminPhone    = "703-930-1674";

    public async Task SeedAsync()
    {
        var existing = await userManager.FindByEmailAsync(AdminEmail);

        if (existing is null)
        {
            logger.LogInformation("[SEED] No super admin found — wiping app data and seeding...");
            await WipeAppDataAsync();
            await CreateAdminAsync();
        }
        else
        {
            // Ensure the admin account stays healthy across restarts
            bool dirty = false;
            if (!existing.IsAdmin)         { existing.IsAdmin = true;       dirty = true; }
            if (!existing.EmailConfirmed)  { existing.EmailConfirmed = true; dirty = true; }
            if (dirty) await userManager.UpdateAsync(existing);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task WipeAppDataAsync()
    {
        // Delete in leaf-to-root dependency order.
        // Lahman tables (lahman schema) are intentionally excluded — never touched.
        await db.Database.ExecuteSqlRawAsync(@"
            DELETE FROM roster_slots;
            DELETE FROM standings;
            DELETE FROM games;
            DELETE FROM sim_batting_stats;
            DELETE FROM sim_pitching_stats;
            DELETE FROM player_cards;
            DELETE FROM teams;
            DELETE FROM league_members;
            DELETE FROM seasons;
            DELETE FROM leagues;
            DELETE FROM ""AspNetUserTokens"";
            DELETE FROM ""AspNetUserLogins"";
            DELETE FROM ""AspNetUserClaims"";
            DELETE FROM ""AspNetUserRoles"";
            DELETE FROM ""AspNetUsers"";
        ");

        logger.LogInformation("[SEED] All app data wiped.");
    }

    private async Task CreateAdminAsync()
    {
        var admin = new ApplicationUser
        {
            Id             = Guid.NewGuid(),
            UserName       = AdminEmail,
            Email          = AdminEmail,
            DisplayName    = AdminDisplay,
            PhoneNumber    = AdminPhone,
            EmailConfirmed = true,
            IsAdmin        = true,
            CreatedAt      = DateTime.UtcNow
        };

        // Create without password first (bypasses password validators).
        var result = await userManager.CreateAsync(admin);
        if (!result.Succeeded)
        {
            logger.LogError("[SEED] Failed to create super admin: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        // Set password hash directly via EF — skips length/complexity validators.
        var hash = userManager.PasswordHasher.HashPassword(admin, AdminPassword);
        await db.Users
            .Where(u => u.Id == admin.Id)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.PasswordHash, hash));

        logger.LogInformation("[SEED] Super admin created: {Email}", AdminEmail);
    }
}
