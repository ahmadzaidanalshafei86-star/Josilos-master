namespace ES.Infrastructure.Configurations
{
    public class PageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            builder.HasOne(e => e.Language)
                      .WithMany()
                      .HasForeignKey(e => e.LanguageId)
                      .OnDelete(DeleteBehavior.NoAction);

            builder.Property(c => c.GalleryStyle)
            .HasConversion<string>();
        }
    }
}
