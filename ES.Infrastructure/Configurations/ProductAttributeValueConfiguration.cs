namespace ES.Infrastructure.Configurations
{
    public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            builder.HasOne(pav => pav.ProductAttribute)
                .WithMany(pa => pa.Values)
                .HasForeignKey(pav => pav.ProductAttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                         .WithMany()
                         .HasForeignKey(e => e.LanguageId)
                         .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
