using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities;

namespace Stratsphere.Data.Configurations;

public class StandingsConfiguration : IEntityTypeConfiguration<Standings>
{
    public void Configure(EntityTypeBuilder<Standings> b)
    {
        b.ToTable("standings");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.TeamId, x.SeasonId }).IsUnique();

        b.Ignore(x => x.GamesPlayed);
        b.Ignore(x => x.WinPct);
        b.Ignore(x => x.RunDiff);

        b.HasOne(x => x.Team).WithMany().HasForeignKey(x => x.TeamId);
        b.HasOne(x => x.Season).WithMany(x => x.Standings).HasForeignKey(x => x.SeasonId);
    }
}
