using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> b)
    {
        b.ToTable("games");
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        b.HasOne(x => x.Season).WithMany(x => x.Games).HasForeignKey(x => x.SeasonId);
        b.HasOne(x => x.HomeTeam).WithMany().HasForeignKey(x => x.HomeTeamId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.AwayTeam).WithMany().HasForeignKey(x => x.AwayTeamId).OnDelete(DeleteBehavior.Restrict);
    }
}
