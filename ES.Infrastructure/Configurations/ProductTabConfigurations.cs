namespace ES.Infrastructure.Configurations
{
    public class ProductTabConfigurations : IEntityTypeConfiguration<ProductTab>
    {
        public void Configure(EntityTypeBuilder<ProductTab> builder)
        {

            builder.HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTabs)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pt => pt.Language)
                  .WithMany()
                  .HasForeignKey(pt => pt.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
