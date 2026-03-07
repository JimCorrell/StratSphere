using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities.Lahman;

namespace StratSphere.Data.Configurations.Lahman;

public class LahmanPitchingConfiguration : IEntityTypeConfiguration<LahmanPitching>
{
    public void Configure(EntityTypeBuilder<LahmanPitching> b)
    {
        b.ToTable("pitching", "lahman");
        b.HasKey(x => new { x.PlayerId, x.YearId, x.Stint });
        b.Property(x => x.PlayerId).HasColumnName("player_id").HasMaxLength(9);
        b.Property(x => x.YearId).HasColumnName("year_id");
        b.Property(x => x.Stint).HasColumnName("stint");
        b.Property(x => x.TeamId).HasColumnName("team_id");
        b.Property(x => x.LgId).HasColumnName("lg_id");
        b.Property(x => x.W).HasColumnName("w");
        b.Property(x => x.L).HasColumnName("l");
        b.Property(x => x.G).HasColumnName("g");
        b.Property(x => x.GS).HasColumnName("gs");
        b.Property(x => x.CG).HasColumnName("cg");
        b.Property(x => x.SHO).HasColumnName("sho");
        b.Property(x => x.SV).HasColumnName("sv");
        b.Property(x => x.IPOuts).HasColumnName("ip_outs");
        b.Property(x => x.H).HasColumnName("h");
        b.Property(x => x.ER).HasColumnName("er");
        b.Property(x => x.HR).HasColumnName("hr");
        b.Property(x => x.BB).HasColumnName("bb");
        b.Property(x => x.SO).HasColumnName("so");
        b.Property(x => x.ERAStored).HasColumnName("era");

        b.Ignore(x => x.IP);
        b.Ignore(x => x.ERA);
        b.Ignore(x => x.WHIP);
        b.Ignore(x => x.K9);
    }
}
