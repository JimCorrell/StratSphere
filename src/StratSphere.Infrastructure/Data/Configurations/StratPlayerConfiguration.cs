using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Infrastructure.Data.Configurations;

public class StratPlayerConfiguration : IEntityTypeConfiguration<StratPlayer>
{
    public void Configure(EntityTypeBuilder<StratPlayer> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PrimaryPosition)
            .HasMaxLength(10);

        builder.Property(p => p.Bats)
            .HasMaxLength(1);

        builder.Property(p => p.Throws)
            .HasMaxLength(1);

        builder.Property(p => p.Status)
            .HasMaxLength(20);

        builder.Property(p => p.QualifyingOffer)
            .HasMaxLength(50);

        builder.Property(p => p.SignedInfo)
            .HasMaxLength(50);

        builder.Property(p => p.Decision)
            .HasMaxLength(20);

        builder.Property(p => p.BaseSalary)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.ContractCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.CurrentSeasonWar)
            .HasColumnType("decimal(5,2)");

        builder.Property(p => p.PreviousSeasonWar)
            .HasColumnType("decimal(5,2)");

        builder.Property(p => p.InningsPitched)
            .HasColumnType("decimal(6,1)");

        builder.Property(p => p.Points)
            .HasColumnType("decimal(10,2)");

        // Relationships
        builder.HasOne(p => p.League)
            .WithMany()
            .HasForeignKey(p => p.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.StratTeam)
            .WithMany(t => t.Players)
            .HasForeignKey(p => p.StratTeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // MLB Player relationship - explicit FK configuration
        builder.HasOne(p => p.MlbPlayer)
            .WithMany()
            .HasForeignKey(p => p.MlbPlayerId)
            .HasPrincipalKey(m => m.PlayerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(p => p.LeagueId);
        builder.HasIndex(p => p.StratTeamId);
        builder.HasIndex(p => p.MlbPlayerId);
        builder.HasIndex(p => p.DisplayName);
        builder.HasIndex(p => p.IsActive);
    }
}
