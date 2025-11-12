namespace ES.Web.Seeds
{
    public class DefaultLanguages
    {
        public static async Task SeedLanguages(ApplicationDbContext context)
        {
            if (!context.Languages.Any())
            {
                await context.Languages.AddRangeAsync(
                    new Language { Name = "English", Code = "en-US" },
                    new Language { Name = "Arabic", Code = "ar-JO" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
