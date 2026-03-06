using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities.Lahman;

namespace Stratsphere.Data.Configurations.Lahman;

public class LahmanFieldingConfiguration : IEntityTypeConfiguration<LahmanFielding>
{
    public void Configure(EntityTypeBuilder<LahmanFielding> b)
    {
        b.ToTable("fielding", "lahman");
        b.HasKey(x => new { x.PlayerId, x.YearId, x.Stint, x.Pos });
        b.Property(x => x.PlayerId).HasColumnName("player_id").HasMaxLength(9);
        b.Property(x => x.YearId).HasColumnName("year_id");
        b.Property(x => x.Stint).HasColumnName("stint");
        b.Property(x => x.TeamId).HasColumnName("team_id");
        b.Property(x => x.LgId).HasColumnName("lg_id");
        b.Property(x => x.Pos).HasColumnName("pos");
        b.Property(x => x.InnOuts).HasColumnName("inn_outs");
        b.Ignore(x => x.FldPct);
    }
}
