using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities.Lahman;

namespace StratSphere.Data.Configurations.Lahman;

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
        b.Property(x => x.G).HasColumnName("g");
        b.Property(x => x.GS).HasColumnName("gs");
        b.Property(x => x.InnOuts).HasColumnName("inn_outs");
        b.Property(x => x.PO).HasColumnName("po");
        b.Property(x => x.A).HasColumnName("a");
        b.Property(x => x.E).HasColumnName("e");
        b.Property(x => x.DP).HasColumnName("dp");
        b.Ignore(x => x.FldPct);
    }
}
