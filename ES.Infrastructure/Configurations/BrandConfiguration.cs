namespace ES.Infrastructure.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasOne(e => e.Language)
                        .WithMany()
                        .HasForeignKey(e => e.LanguageId)
                        .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
