using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> b)
    {
        b.ToTable("leagues");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.Property(x => x.Slug).HasMaxLength(60).IsRequired();
        b.HasIndex(x => x.Slug).IsUnique();
        b.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("setup");

        b.HasOne(x => x.Commissioner)
         .WithMany()
         .HasForeignKey(x => x.CommissionerId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Members)
         .WithOne(x => x.League)
         .HasForeignKey(x => x.LeagueId);
    }
}
