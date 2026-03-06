using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities.Lahman;

namespace Stratsphere.Data.Configurations.Lahman;

public class LahmanBattingConfiguration : IEntityTypeConfiguration<LahmanBatting>
{
    public void Configure(EntityTypeBuilder<LahmanBatting> b)
    {
        b.ToTable("batting", "lahman");
        b.HasKey(x => new { x.PlayerId, x.YearId, x.Stint });
        b.Property(x => x.PlayerId).HasColumnName("player_id").HasMaxLength(9);
        b.Property(x => x.YearId).HasColumnName("year_id");
        b.Property(x => x.Stint).HasColumnName("stint");
        b.Property(x => x.TeamId).HasColumnName("team_id");
        b.Property(x => x.LgId).HasColumnName("lg_id");
        b.Property(x => x.Doubles).HasColumnName("doubles");
        b.Property(x => x.GIDP).HasColumnName("g_idp");

        b.Ignore(x => x.BA);
        b.Ignore(x => x.OBP);
        b.Ignore(x => x.SLG);
        b.Ignore(x => x.OPS);
    }
}
