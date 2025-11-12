namespace ES.Infrastructure.Configurations
{
    public class FieldTranslationConfiguration : IEntityTypeConfiguration<FieldTranslation>
    {
        public void Configure(EntityTypeBuilder<FieldTranslation> builder)
        {
            builder.HasOne(ft => ft.Field)
                 .WithMany(f => f.Translations)
                 .HasForeignKey(ft => ft.FieldId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ft => ft.Language)
                .WithMany()
                .HasForeignKey(ft => ft.LanguageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(t => new { t.FieldId, t.LanguageId })
            .IsUnique();
        }
    }
}
