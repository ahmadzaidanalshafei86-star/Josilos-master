namespace ES.Infrastructure.Configurations
{
    public class ProductTabTranslationsConfigurations : IEntityTypeConfiguration<ProductTabTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTabTranslation> builder)
        {
            builder.HasOne(ptt => ptt.ProductTab)
            .WithMany(pt => pt.Translations)
            .HasForeignKey(ptt => ptt.ProductTabId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ptt => ptt.Language)
                 .WithMany()
                 .HasForeignKey(ptt => ptt.LanguageId)
                 .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
