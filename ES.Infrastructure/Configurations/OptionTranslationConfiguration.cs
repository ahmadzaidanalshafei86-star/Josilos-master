namespace ES.Infrastructure.Configurations
{
    public class OptionTranslationConfiguration : IEntityTypeConfiguration<OptionTranslation>
    {
        public void Configure(EntityTypeBuilder<OptionTranslation> builder)
        {
            builder.HasOne(ot => ot.Option)
               .WithMany(o => o.Translations)
               .HasForeignKey(ot => ot.OptionId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ot => ot.Language)
               .WithMany()
               .HasForeignKey(ot => ot.LanguageId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(t => new { t.OptionId, t.LanguageId })
                .IsUnique();
        }
    }
}
