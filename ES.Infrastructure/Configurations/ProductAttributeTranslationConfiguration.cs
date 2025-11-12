namespace ES.Infrastructure.Configurations
{
    public class ProductAttributeTranslationConfiguration : IEntityTypeConfiguration<ProductAttributeTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeTranslation> builder)
        {
            builder.HasOne(e => e.ProductAttribute)
                   .WithMany(e => e.Translations)
                   .HasForeignKey(e => e.ProductAttributeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
