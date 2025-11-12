namespace ES.Infrastructure.Configurations
{
    public class FormResponseConfiguration : IEntityTypeConfiguration<FormResponse>
    {
        public void Configure(EntityTypeBuilder<FormResponse> builder)
        {
            builder.HasOne(fr => fr.Form)
                 .WithMany()
                 .HasForeignKey(fr => fr.FormId)
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
