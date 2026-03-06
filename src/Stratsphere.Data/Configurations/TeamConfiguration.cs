using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities;

namespace Stratsphere.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> b)
    {
        b.ToTable("teams");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(100);
        b.Property(x => x.City).HasMaxLength(100);
        b.Property(x => x.Abbreviation).HasMaxLength(3);

        b.HasOne(x => x.League).WithMany(x => x.Teams).HasForeignKey(x => x.LeagueId);
        b.HasOne(x => x.User).WithMany(x => x.Teams).HasForeignKey(x => x.UserId);
        b.HasOne(x => x.Season).WithMany(x => x.Teams).HasForeignKey(x => x.SeasonId);
    }
}
