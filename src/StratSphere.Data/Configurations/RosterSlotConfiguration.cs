using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class RosterSlotConfiguration : IEntityTypeConfiguration<RosterSlot>
{
    public void Configure(EntityTypeBuilder<RosterSlot> b)
    {
        b.ToTable("roster_slots");
        b.HasKey(x => x.Id);
        b.Property(x => x.SlotType).HasMaxLength(20).HasDefaultValue("active");

        // Partial unique index: a card can only be on one active roster per season.
        // DroppedAt == null slots are unique; historical (dropped) slots are not constrained,
        // allowing a card to be re-acquired after being dropped.
        b.HasIndex(x => new { x.SeasonId, x.CardId })
            .IsUnique()
            .HasFilter("\"DroppedAt\" IS NULL")
            .HasDatabaseName("IX_roster_slots_active_card_season");

        b.HasOne(x => x.Team).WithMany(x => x.RosterSlots).HasForeignKey(x => x.TeamId);
        b.HasOne(x => x.Season).WithMany(x => x.RosterSlots).HasForeignKey(x => x.SeasonId);
        b.HasOne(x => x.Card).WithMany(x => x.RosterSlots).HasForeignKey(x => x.CardId);
    }
}
