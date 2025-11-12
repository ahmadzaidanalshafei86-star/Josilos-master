namespace ES.Infrastructure.Configurations
{
    public class ProductLabelConfiguration : IEntityTypeConfiguration<ProductLabel>
    {
        public void Configure(EntityTypeBuilder<ProductLabel> builder)
        {
            builder.HasOne(e => e.Language)
                        .WithMany()
                        .HasForeignKey(e => e.LanguageId)
                        .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
