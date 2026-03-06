using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities;

namespace Stratsphere.Data.Configurations;

public class LeagueMemberConfiguration : IEntityTypeConfiguration<LeagueMember>
{
    public void Configure(EntityTypeBuilder<LeagueMember> b)
    {
        b.ToTable("league_members");
        b.HasKey(x => new { x.LeagueId, x.UserId });
        b.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
    }
}
