namespace ES.Infrastructure.Configurations
{

    public class ProductLabelTranslationConfiguration : IEntityTypeConfiguration<ProductLabelTranslate>
    {
        public void Configure(EntityTypeBuilder<ProductLabelTranslate> builder)
        {
            builder.HasOne(e => e.ProductLabel)
                   .WithMany(e => e.ProductLabelTranslate)
                   .HasForeignKey(e => e.ProductLabelId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
