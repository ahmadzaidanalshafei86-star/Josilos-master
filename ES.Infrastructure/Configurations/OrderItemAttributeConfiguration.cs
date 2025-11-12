namespace ES.Infrastructure.Configurations
{
    public class OrderItemAttributeConfiguration : IEntityTypeConfiguration<OrderItemAttribute>
    {
        public void Configure(EntityTypeBuilder<OrderItemAttribute> builder)
        {
            builder.HasOne<OrderItem>()
                .WithMany(oi => oi.SelectedAttributes)
                .HasForeignKey(a => a.SelectedOrderItemId)
                .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
