using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities.Lahman;

namespace StratSphere.Data.Configurations.Lahman;

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
        b.Property(x => x.G).HasColumnName("g");
        b.Property(x => x.AB).HasColumnName("ab");
        b.Property(x => x.R).HasColumnName("r");
        b.Property(x => x.H).HasColumnName("h");
        b.Property(x => x.Doubles).HasColumnName("doubles");
        b.Property(x => x.Triples).HasColumnName("triples");
        b.Property(x => x.HR).HasColumnName("hr");
        b.Property(x => x.RBI).HasColumnName("rbi");
        b.Property(x => x.SB).HasColumnName("sb");
        b.Property(x => x.CS).HasColumnName("cs");
        b.Property(x => x.BB).HasColumnName("bb");
        b.Property(x => x.SO).HasColumnName("so");
        b.Property(x => x.IBB).HasColumnName("ibb");
        b.Property(x => x.HBP).HasColumnName("hbp");
        b.Property(x => x.SH).HasColumnName("sh");
        b.Property(x => x.SF).HasColumnName("sf");
        b.Property(x => x.GIDP).HasColumnName("g_idp");

        b.Ignore(x => x.BA);
        b.Ignore(x => x.OBP);
        b.Ignore(x => x.SLG);
        b.Ignore(x => x.OPS);
    }
}
