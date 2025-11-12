namespace ES.Infrastructure.Configurations
{
    public class ProductAttributeValueTranslationConfiguration : IEntityTypeConfiguration<ProductAttributeValueTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValueTranslation> builder)
        {
            builder.HasOne(pavt => pavt.ProductAttributeValue)
                   .WithMany(pav => pav.Translations)
                   .HasForeignKey(pavt => pavt.ProductAttributeValueId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
