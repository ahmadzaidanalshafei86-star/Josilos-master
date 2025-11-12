namespace ES.Infrastructure.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.HasOne(e => e.Language)
                         .WithMany()
                         .HasForeignKey(e => e.LanguageId)
                         .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
