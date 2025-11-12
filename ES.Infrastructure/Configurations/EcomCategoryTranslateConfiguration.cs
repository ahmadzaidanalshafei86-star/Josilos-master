namespace ES.Infrastructure.Configurations
{
    public class EcomCategoryTranslateConfiguration : IEntityTypeConfiguration<EcomCategoryTranslate>
    {
        public void Configure(EntityTypeBuilder<EcomCategoryTranslate> builder)
        {
            builder.HasOne(e => e.EcomCategory)
                     .WithMany(e => e.CategoryTranslates)
                     .HasForeignKey(e => e.EcomCategoryId)
                     .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
