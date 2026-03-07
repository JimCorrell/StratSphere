using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> b)
    {
        b.ToTable("seasons");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(50);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        b.HasOne(x => x.League)
         .WithMany(x => x.Seasons)
         .HasForeignKey(x => x.LeagueId);
    }
}
