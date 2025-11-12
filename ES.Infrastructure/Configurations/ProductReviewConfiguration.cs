namespace ES.Infrastructure.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasOne(pr => pr.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(pr => pr.ProductId);

            builder.HasOne(pr => pr.FormResponse)
            .WithOne()
            .HasForeignKey<ProductReview>(ca => ca.FormResponseId);
        }
    }
}
