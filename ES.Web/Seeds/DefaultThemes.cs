namespace ES.Web.Seeds
{
    public class DefaultThemes
    {
        public static async Task SeedThemes(ApplicationDbContext context)
        {
            if (!context.Themes.Any())
            {
                await context.Themes.AddRangeAsync(
                    new Theme { ThemeName = "grid-1" },
                    new Theme { ThemeName = "grid-2" },
                    new Theme { ThemeName = "list-1" },
                    new Theme { ThemeName = "list-2" },
                    new Theme { ThemeName = "news-1" },
                    new Theme { ThemeName = "news-2" },
                    new Theme { ThemeName = "blogs" },
                    new Theme { ThemeName = "tabs" },
                    new Theme { ThemeName = "faqs" },
                    new Theme { ThemeName = "video" }
                );
                await context.SaveChangesAsync();
            }

            //if (context.Themes.Any()){
            //    var theme = await context.Themes.FirstOrDefaultAsync(t => t.ThemeName == "news-2");
            //    context.Remove(theme);
            //    await context.SaveChangesAsync();
            //}
        }
    }
}
