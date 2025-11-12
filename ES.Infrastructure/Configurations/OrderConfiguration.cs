namespace ES.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");
            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.EstimatedTotal)
                .HasColumnType("decimal(18,2)");

        }

    }
}
