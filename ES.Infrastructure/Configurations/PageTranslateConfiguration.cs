namespace ES.Infrastructure.Configurations
{
    public class PageTranslateConfiguration : IEntityTypeConfiguration<PageTranslate>
    {
        public void Configure(EntityTypeBuilder<PageTranslate> builder)
        {
            builder.HasOne(e => e.Page)
                   .WithMany(e => e.PageTranslates)
                   .HasForeignKey(e => e.PageId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
