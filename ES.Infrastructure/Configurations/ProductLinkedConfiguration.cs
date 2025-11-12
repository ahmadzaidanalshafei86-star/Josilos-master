namespace ES.Infrastructure.Configurations
{
    public class ProductLinkedConfiguration : IEntityTypeConfiguration<ProductLinked>
    {
        public void Configure(EntityTypeBuilder<ProductLinked> builder)
        {
            builder.HasKey(pl => new { pl.ProductId, pl.LinkedProductId });

            builder.HasOne(pl => pl.Product)
            .WithMany(p => p.LinkedProducts)
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(pl => pl.LinkedProduct)
             .WithMany(p => p.LinkedToProducts)
             .HasForeignKey(pl => pl.LinkedProductId)
              .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
