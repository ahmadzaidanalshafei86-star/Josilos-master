namespace ES.Infrastructure.Configurations
{
    public class PageCategoryConfiguration : IEntityTypeConfiguration<PageCategory>
    {
        public void Configure(EntityTypeBuilder<PageCategory> builder)
        {
            builder.HasKey(pc => new { pc.PageId, pc.CategoryId });
        }
    }

}
