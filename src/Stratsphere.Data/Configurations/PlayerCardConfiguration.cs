using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class PlayerCardConfiguration : IEntityTypeConfiguration<PlayerCard>
{
    public void Configure(EntityTypeBuilder<PlayerCard> b)
    {
        b.ToTable("player_cards");
        b.HasKey(x => x.Id);
        b.Property(x => x.LahmanPlayerId).HasMaxLength(9).IsRequired();
        b.Property(x => x.Position).HasMaxLength(5).IsRequired();
        b.HasIndex(x => new { x.LahmanPlayerId, x.CardYear, x.Position }).IsUnique();
    }
}
