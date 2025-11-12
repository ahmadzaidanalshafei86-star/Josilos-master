namespace ES.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure the many-to-many relationship
            builder.HasMany(c => c.RelatedCategories)
                .WithMany(c => c.CategoriesRelatedToThis)
                .UsingEntity(j => j.ToTable("CategoryRelationships"));

            builder.Property(c => c.TypeOfSorting)
            .HasConversion<string>();


            builder.Property(c => c.GalleryStyle)
                .HasConversion<string>();
        }

    }
}
