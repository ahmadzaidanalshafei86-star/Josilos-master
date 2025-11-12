namespace ES.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(e => e.Language)
                       .WithMany()
                       .HasForeignKey(e => e.LanguageId)
                       .OnDelete(DeleteBehavior.NoAction);

            builder.Property(p => p.ProductType)
            .HasConversion<string>();

            builder.Property(p => p.AllowBackorders)
            .HasConversion<string>();

            builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.ProductLabel)
                .WithMany(pl => pl.Products)
                .HasForeignKey(p => p.ProductLabelId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.ProductTabs)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
