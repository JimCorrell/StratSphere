using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StratSphere.Core.Entities;

namespace StratSphere.Data.Configurations;

public class SomImportLogConfiguration : IEntityTypeConfiguration<SomImportLog>
{
    public void Configure(EntityTypeBuilder<SomImportLog> b)
    {
        b.ToTable("som_import_logs");
        b.HasKey(x => x.Id);
        b.HasOne(x => x.Season).WithMany().HasForeignKey(x => x.SeasonId).OnDelete(DeleteBehavior.Cascade);
    }
}
