using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities.Lahman;

namespace Stratsphere.Data.Configurations.Lahman;

/// <summary>
/// Maps to the lahman schema. Read-only — excluded from migrations.
/// </summary>
public class LahmanPersonConfiguration : IEntityTypeConfiguration<LahmanPerson>
{
    public void Configure(EntityTypeBuilder<LahmanPerson> b)
    {
        b.ToTable("people", "lahman");
        b.HasKey(x => x.PlayerId);
        b.Property(x => x.PlayerId).HasColumnName("player_id").HasMaxLength(9);
        b.Property(x => x.NameFirst).HasColumnName("name_first");
        b.Property(x => x.NameLast).HasColumnName("name_last");
        b.Property(x => x.NameGiven).HasColumnName("name_given");
        b.Property(x => x.BirthYear).HasColumnName("birth_year");
        b.Property(x => x.BirthMonth).HasColumnName("birth_month");
        b.Property(x => x.BirthDay).HasColumnName("birth_day");
        b.Property(x => x.Bats).HasColumnName("bats").HasMaxLength(1);
        b.Property(x => x.Throws).HasColumnName("throws").HasMaxLength(1);
        b.Property(x => x.Debut).HasColumnName("debut");
        b.Property(x => x.FinalGame).HasColumnName("final_game");
        b.Property(x => x.BbrefId).HasColumnName("bbref_id");
        b.Property(x => x.RetroId).HasColumnName("retro_id");
        b.Ignore(x => x.FullName);
    }
}
