namespace ES.Infrastructure.Configurations
{
    public class FormResponseDetailConfiguration : IEntityTypeConfiguration<FormResponseDetail>
    {
        public void Configure(EntityTypeBuilder<FormResponseDetail> builder)
        {
            builder.HasOne(frd => frd.Response)
                 .WithMany(fr => fr.ResponseDetails)
                 .HasForeignKey(frd => frd.ResponseId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(frd => frd.Field)
                .WithMany()
                .HasForeignKey(frd => frd.FieldId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
