namespace ES.Infrastructure.Configurations
{
    public class CareerApplicationConfiguration : IEntityTypeConfiguration<CareerApplication>
    {
        public void Configure(EntityTypeBuilder<CareerApplication> builder)
        {
            builder.HasOne(ca => ca.Career)
            .WithMany(c => c.Applications)
            .HasForeignKey(ca => ca.CareerId);

            builder.HasOne(ca => ca.FormResponse)
            .WithOne()
            .HasForeignKey<CareerApplication>(ca => ca.FormResponseId);
        }
    }
}
