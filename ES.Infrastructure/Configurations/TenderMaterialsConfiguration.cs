using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ES.Core.Entities;

namespace ES.Infrastructure.Configurations
{
    public class TenderMaterialsConfiguration : IEntityTypeConfiguration<TenderMaterial>
    {
        public void Configure(EntityTypeBuilder<TenderMaterial> builder)
        {
            builder.HasKey(tm => new { tm.TenderId, tm.MaterialId });

            builder
                .HasOne(tm => tm.Tender)
                .WithMany(t => t.Materials)
                .HasForeignKey(tm => tm.TenderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(tm => tm.Material)
                .WithMany(m => m.Tenders)
                .HasForeignKey(tm => tm.MaterialId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
