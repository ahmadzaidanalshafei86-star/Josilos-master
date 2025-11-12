namespace ES.Infrastructure.Configurations
{
    public class RowLevelPermissionConfiguration : IEntityTypeConfiguration<RowLevelPermission>
    {
        public void Configure(EntityTypeBuilder<RowLevelPermission> builder)
        {
            builder.HasOne(rlp => rlp.Role)
                   .WithMany()
                   .HasForeignKey(rlp => rlp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade); // Adjust the delete behavior as needed

        }
    }
}



