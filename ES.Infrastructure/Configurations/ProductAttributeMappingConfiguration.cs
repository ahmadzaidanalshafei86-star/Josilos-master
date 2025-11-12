namespace ES.Infrastructure.Configurations
{
    public class ProductAttributeMappingConfiguration : IEntityTypeConfiguration<ProductAttributeMapping>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeMapping> builder)
        {
            builder.HasIndex(p => new { p.ProductId, p.ProductAttributeId, p.ProductAttributeValueId })
                   .IsUnique();


            builder.HasOne(pam => pam.Product)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(pam => pam.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pam => pam.ProductAttribute)
                .WithMany()
                .HasForeignKey(pam => pam.ProductAttributeId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(pam => pam.ProductAttributeValue)
                .WithMany()
                .HasForeignKey(pam => pam.ProductAttributeValueId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
