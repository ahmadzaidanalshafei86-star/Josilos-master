

namespace ES.Infrastructure.Configurations
{
    public class CareerTranslatesConfiguration : IEntityTypeConfiguration<CareerTranslate>
    {
        public void Configure(EntityTypeBuilder<CareerTranslate> builder)
        {
            builder.HasOne(e => e.Career)
                   .WithMany(e => e.CareerTranslates)
                   .HasForeignKey(e => e.CareerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Language)
                  .WithMany()
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
