using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Stratsphere.Core.Entities;
using Stratsphere.Core.Entities.Lahman;

namespace Stratsphere.Data;

public class StratosphereDbContext(DbContextOptions<StratosphereDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    // ── App tables (public schema) ────────────────────────────────────────────
    public DbSet<League>           Leagues          { get; set; }
    public DbSet<LeagueMember>     LeagueMembers    { get; set; }
    public DbSet<Season>           Seasons          { get; set; }
    public DbSet<Team>             Teams            { get; set; }
    public DbSet<PlayerCard>       PlayerCards      { get; set; }
    public DbSet<RosterSlot>       RosterSlots      { get; set; }
    public DbSet<SimBattingStats>  SimBattingStats  { get; set; }
    public DbSet<SimPitchingStats> SimPitchingStats { get; set; }
    public DbSet<Game>             Games            { get; set; }
    public DbSet<Standings>        Standings        { get; set; }

    // ── Lahman tables (lahman schema, read-only) ──────────────────────────────
    public DbSet<LahmanPerson>   LahmanPeople   { get; set; }
    public DbSet<LahmanBatting>  LahmanBatting  { get; set; }
    public DbSet<LahmanPitching> LahmanPitching { get; set; }
    public DbSet<LahmanFielding> LahmanFielding { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(StratosphereDbContext).Assembly);
    }
}
