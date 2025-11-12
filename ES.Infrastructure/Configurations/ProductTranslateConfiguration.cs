namespace ES.Infrastructure.Configurations
{
    public class ProductTranslateConfiguration : IEntityTypeConfiguration<ProductTranslate>
    {
        public void Configure(EntityTypeBuilder<ProductTranslate> builder)
        {
            builder.HasOne(e => e.Product)
                        .WithMany(e => e.ProductTranslates)
                        .HasForeignKey(e => e.ProductId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
