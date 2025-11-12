namespace ES.Infrastructure.Configurations
{
    public class MenuItemTranslateConfiguration : IEntityTypeConfiguration<MenuItemTranslate>
    {
        public void Configure(EntityTypeBuilder<MenuItemTranslate> builder)
        {
            builder.HasOne(mt => mt.MenuItem)
               .WithMany(m => m.Translations)
               .HasForeignKey(mt => mt.MenuItemId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
