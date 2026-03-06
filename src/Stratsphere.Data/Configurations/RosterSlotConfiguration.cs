using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stratsphere.Core.Entities;

namespace Stratsphere.Data.Configurations;

public class RosterSlotConfiguration : IEntityTypeConfiguration<RosterSlot>
{
    public void Configure(EntityTypeBuilder<RosterSlot> b)
    {
        b.ToTable("roster_slots");
        b.HasKey(x => x.Id);
        b.Property(x => x.SlotType).HasMaxLength(20).HasDefaultValue("active");

        // A card can only be on one team per season
        b.HasIndex(x => new { x.SeasonId, x.CardId }).IsUnique();

        b.HasOne(x => x.Team).WithMany(x => x.RosterSlots).HasForeignKey(x => x.TeamId);
        b.HasOne(x => x.Season).WithMany(x => x.RosterSlots).HasForeignKey(x => x.SeasonId);
        b.HasOne(x => x.Card).WithMany(x => x.RosterSlots).HasForeignKey(x => x.CardId);
    }
}
