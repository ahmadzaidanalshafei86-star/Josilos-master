namespace ES.Infrastructure.Configurations
{
    public class CareerConfiguration : IEntityTypeConfiguration<Career>
    {
        public void Configure(EntityTypeBuilder<Career> builder)
        {
            builder.HasOne(e => e.Language)
                        .WithMany()
                        .HasForeignKey(e => e.LanguageId)
                        .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
