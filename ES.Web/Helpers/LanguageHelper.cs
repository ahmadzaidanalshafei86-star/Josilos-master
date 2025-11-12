namespace ES.Web.Helpers
{
    public static class LanguageHelper
    {
        public static async Task<int?> GetCurrentLanguageIdAsync(ApplicationDbContext context)
        {
            var currentCultureCode = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            var language = await context.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Code.StartsWith(currentCultureCode));

            return language?.Id;
        }
    }
}
