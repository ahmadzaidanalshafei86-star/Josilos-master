namespace ES.Infrastructure.Configurations
{
    public class BrandTranslateConfiguration : IEntityTypeConfiguration<BrandTranslate>
    {
        public void Configure(EntityTypeBuilder<BrandTranslate> builder)
        {
            builder.HasOne(e => e.Brand)
                   .WithMany(e => e.BrandTranslates)
                   .HasForeignKey(e => e.BrandId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
