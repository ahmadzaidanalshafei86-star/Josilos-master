using System.Text.RegularExpressions;

namespace ES.Web.Areas.EsAdmin.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string input)
        {
            input = input.ToLowerInvariant();
            input = Regex.Replace(input, @"[^a-z0-9\s-]", ""); // Remove invalid chars
            input = Regex.Replace(input, @"\s+", "-"); // Replace spaces with hyphens
            return input.Trim('-');
        }
    }
}
