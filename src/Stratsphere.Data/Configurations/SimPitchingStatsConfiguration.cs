using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class SimPitchingStatsConfiguration : IEntityTypeConfiguration<SimPitchingStats>
{
    public void Configure(EntityTypeBuilder<SimPitchingStats> b)
    {
        b.ToTable("sim_pitching_stats");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.CardId, x.SeasonId }).IsUnique();

        b.Ignore(x => x.IP);
        b.Ignore(x => x.ERA);
        b.Ignore(x => x.WHIP);
        b.Ignore(x => x.K9);

        b.HasOne(x => x.Card).WithMany(x => x.SimPitchingStats).HasForeignKey(x => x.CardId);
        b.HasOne(x => x.Season).WithMany().HasForeignKey(x => x.SeasonId);
        b.HasOne(x => x.Team).WithMany().HasForeignKey(x => x.TeamId);
    }
}
