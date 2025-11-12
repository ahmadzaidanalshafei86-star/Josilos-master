namespace ES.Infrastructure.Configurations
{
    public class CategoryTranslateConfiguration : IEntityTypeConfiguration<CategoryTranslate>
    {
        public void Configure(EntityTypeBuilder<CategoryTranslate> builder)
        {
            builder.HasOne(e => e.Category)
                    .WithMany(e => e.CategoryTranslates)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
