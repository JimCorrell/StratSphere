using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Infrastructure.Data.Configurations;

public class StratTeamConfiguration : IEntityTypeConfiguration<StratTeam>
{
    public void Configure(EntityTypeBuilder<StratTeam> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Abbreviation)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Owner)
            .HasMaxLength(100);

        builder.Property(t => t.PrimaryColor)
            .HasMaxLength(7);

        builder.Property(t => t.SecondaryColor)
            .HasMaxLength(7);

        builder.Property(t => t.Division)
            .HasMaxLength(50);

        builder.Property(t => t.TotalSalary)
            .HasColumnType("decimal(18,2)");

        // Relationships
        builder.HasOne(t => t.League)
            .WithMany()
            .HasForeignKey(t => t.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Players)
            .WithOne(p => p.StratTeam)
            .HasForeignKey(p => p.StratTeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(t => t.LeagueId);
        builder.HasIndex(t => t.Abbreviation);
        builder.HasIndex(t => t.IsActive);
        builder.HasIndex(t => new { t.LeagueId, t.Abbreviation })
            .IsUnique()
            .HasFilter("\"IsActive\" = true");
    }
}
