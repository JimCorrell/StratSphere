using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class SimBattingStatsConfiguration : IEntityTypeConfiguration<SimBattingStats>
{
    public void Configure(EntityTypeBuilder<SimBattingStats> b)
    {
        b.ToTable("sim_batting_stats");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.CardId, x.SeasonId }).IsUnique();

        // Computed properties are not mapped to columns
        b.Ignore(x => x.BA);
        b.Ignore(x => x.OBP);
        b.Ignore(x => x.SLG);
        b.Ignore(x => x.OPS);

        b.HasOne(x => x.Card).WithMany(x => x.SimBattingStats).HasForeignKey(x => x.CardId);
        b.HasOne(x => x.Season).WithMany().HasForeignKey(x => x.SeasonId);
        b.HasOne(x => x.Team).WithMany().HasForeignKey(x => x.TeamId);
    }
}
