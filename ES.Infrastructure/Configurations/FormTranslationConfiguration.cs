namespace ES.Infrastructure.Configurations
{
    public class FormTranslationConfiguration : IEntityTypeConfiguration<FormTranslation>
    {
        public void Configure(EntityTypeBuilder<FormTranslation> builder)
        {
            builder.HasOne(ft => ft.Form)
               .WithMany(ft => ft.Translations)
               .HasForeignKey(ft => ft.FormId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ft => ft.Language)
                .WithMany()
                .HasForeignKey(ft => ft.LanguageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(t => new { t.FormId, t.LanguageId })
            .IsUnique();
        }
    }
}
